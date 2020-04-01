using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
using Crestron.SimplSharp.Reflection;
#else
using System.Data;
using System.Reflection;
#endif

namespace ICD.Connect.Settings.ORM.Extensions
{
	public static class DbConnectionExtensions
	{
		#region Methods

		/// <summary>
		/// Overload to allow a basic execute call.
		/// </summary>
		/// <returns>Number of rows affected.</returns>
		public static int Execute(this IDbConnection cnn, string sql)
		{
			return cnn.Execute(sql, null, null);
		}

		/// <summary>
		/// Executes an SQL statement and returns the number of rows affected. Supports transactions.
		/// </summary>
		/// <returns>Number of rows affected.</returns>
		public static int Execute(this IDbConnection cnn, string sql, object param, IDbTransaction transaction)
		{
			using (IDbCommand cmd = SetupCommand(cnn, transaction, sql, null, null))
			{
				AddParams(cmd, param);
				return cmd.ExecuteNonQuery();
			}
		}

		public static object ExecuteScalar(this IDbConnection cnn, string sql)
		{
			return cnn.ExecuteScalar(sql, null, null);
		}

		public static object ExecuteScalar(this IDbConnection cnn, string sql, object param, IDbTransaction transaction)
		{
			using (IDbCommand cmd = SetupCommand(cnn, transaction, sql, null, null))
			{
				AddParams(cmd, param);
				return cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Returns all of the rows for the given SQL query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="conn"></param>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static IEnumerable<T> Query<T>(this IDbConnection conn, string sql)
		{
			return Query<T>(conn, sql, null);
		}

		/// <summary>
		/// Returns all of the rows for the given SQL query, injecting the given parameters into the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="conn"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public static IEnumerable<T> Query<T>(this IDbConnection conn, string sql, object param)
		{
			using (IDbCommand cmd = SetupCommand(conn, null, sql, null, null))
			{
				AddParams(cmd, param);

				using (IDataReader reader = cmd.ExecuteReader())
				{
					if (reader == null)
						return Enumerable.Empty<T>();

					Type type = typeof(T);
					if (type.IsValueType || type == typeof(string))
						return ReadValues<T>(reader).ToArray();

					return ReadReferences<T>(reader).ToArray();
				}
			}
		}

		/// <summary>
		/// Returns all records in the given table.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static IEnumerable<T> All<T>(this IDbConnection connection, string tableName)
		{
			return connection.All<T>(tableName, new {});
		}

		/// <summary>
		/// Returns all of the records in the given table matching the given parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="tableName"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public static IEnumerable<T> All<T>(this IDbConnection connection, string tableName, object param)
		{
			TypeModel typeModel = TypeModel.Get(typeof(T));
			TypeModel paramModel = TypeModel.Get(param.GetType());

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("SELECT ");
				builder.Append(string.Join(",", typeModel.GetPropertyNames().ToArray()));
				builder.Append(" FROM ");
				builder.Append(tableName);

				string[] properties = paramModel.GetPropertyNames().ToArray();
				if (properties.Length != 0)
					builder.Append(" WHERE ");

				for (int index = 0; index < properties.Length; index++)
				{
					if (index != 0)
						builder.Append(" AND ");

					string property = properties[index];
					builder.AppendFormat("{0}=@{0}", property);
				}
			}
			string sql = builder.ToString();

			return connection.Query<T>(sql, param);
		}

		/// <summary>
		/// Inserts the given parameters into the given table.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <param name="tableName"></param>
		/// <param name="param"></param>
		public static void Insert<T>(this IDbConnection connection, IDbTransaction transaction, string tableName, object param)
		{
			TypeModel typeModel = TypeModel.Get(typeof(T));
			TypeModel paramModel = TypeModel.Get(param.GetType());

			string[] properties =
				paramModel.GetPropertyNames()
				          // Don't try to insert the primary key if it auto-increments
				          .Where(p => !(p == typeModel.PrimaryKeyName && typeModel.AutoIncrements))
				          .ToArray();

			// "auto-increment" behaviour for Guid ids
			AutoIncrementGuid(typeModel, paramModel, param);

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("INSERT INTO ").Append(tableName);

				// Columns
				builder.Append("(");
				builder.Append(string.Join(",", properties));
				builder.Append(")");

				builder.Append(" VALUES ");

				// Values
				builder.Append("(");
				builder.Append(string.Join(",", properties.Select(p => string.Format("@{0}", p)).ToArray()));
				builder.Append(")");
			}
			string sql = builder.ToString();

			int result = connection.Execute(sql, param, transaction);
			if (result <= 0)
				throw new ApplicationException("Return value of INSERT should be greater than 0. An error has occurred with the INSERT.");

			// Assign the inserted ID back onto the param
			QueryLastId(connection, tableName, typeModel, paramModel, param);
		}

		/// <summary>
		/// Updates the record matching the given primary key with the given parameters.
		/// </summary>
		public static void Update<T>(this IDbConnection connection, IDbTransaction transaction, string tableName, object param)
		{
			TypeModel typeModel = TypeModel.Get(typeof(T));
			TypeModel paramModel = TypeModel.Get(param.GetType());

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("UPDATE ").Append(tableName).Append(" SET ");
				builder.AppendLine(string.Join(",", paramModel.GetPropertyNames()
				                                              .Where(n => n != typeModel.PrimaryKeyName)
				                                              .Select(p => p + "= @" + p)
				                                              .ToArray()));

				builder.Append(string.Format(" WHERE {0} = @{0}", typeModel.PrimaryKeyName));
			}
			string sql = builder.ToString();

			int result = connection.Execute(sql, param, transaction);
			if (result <= 0)
				throw new ApplicationException("Return value of UPDATE should be greater than 0. An error has occurred with the UPDATE.");
		}

		/// <summary>
		/// Deletes the records matching the given parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <param name="tableName"></param>
		/// <param name="param"></param>
		public static void Delete<T>(this IDbConnection connection, IDbTransaction transaction, string tableName, object param)
		{
			TypeModel paramModel = TypeModel.Get(param.GetType());

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("DELETE FROM ").Append(tableName);

				string[] properties = paramModel.GetPropertyNames().ToArray();
				if (properties.Length != 0)
					builder.Append(" WHERE ");

				for (int index = 0; index < properties.Length; index++)
				{
					if (index != 0)
						builder.Append(" AND ");

					string property = properties[index];
					builder.AppendFormat("{0}=@{0}", property);
				}
			}
			string sql = builder.ToString();

			int result = connection.Execute(sql, param, transaction);
			if (result < 0)
				throw new ApplicationException("Return value of DELETE should be greater than or equal to 0. An error has occurred with the DELETE.");
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reads the rows in the reader as the given value type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static IEnumerable<T> ReadValues<T>(IDataReader reader)
		{
			while (reader.Read())
			{
				// Handles the case where the value is null.
				if (reader.IsDBNull(0))
					yield return default(T);
				else
					yield return (T)reader[0];
			}
		}

		/// <summary>
		/// Reads the rows in the reader as the given reference type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static IEnumerable<T> ReadReferences<T>(IDataReader reader)
		{
			while (reader.Read())
			{
				T record = ReflectionUtils.CreateInstance<T>();
				PopulateClass(record, reader);
				yield return record;
			}
		}

		/// <summary>
		/// Pulled straight from the Dapper source.
		/// </summary>
		private static IDbCommand SetupCommand(IDbConnection cnn, IDbTransaction transaction, string sql, int? commandTimeout,
		                                       CommandType? commandType)
		{
			IDbCommand cmd = cnn.CreateCommand();

			if (transaction != null)
				cmd.Transaction = transaction;

			cmd.CommandText = sql;

			if (commandTimeout.HasValue)
				cmd.CommandTimeout = commandTimeout.Value;

			if (commandType.HasValue)
				cmd.CommandType = commandType.Value;

			return cmd;
		}

		/// <summary>
		/// Inject parameters from the supplied object into the command object.
		/// </summary>
		private static void AddParams(IDbCommand cmd, object data)
		{
			if (cmd == null || data == null)
				return;

			TypeModel typeModel = TypeModel.Get(data.GetType());

			foreach (string propertyName in typeModel.GetPropertyNames())
			{
				IDbDataParameter param = cmd.CreateParameter();
				{
					param.ParameterName = "@" + propertyName;
					param.DbType = typeModel.GetPropertyDbType(propertyName);
					param.Value = typeModel.GetPropertyValue(data, propertyName) ?? DBNull.Value;
				}

#if SIMPLSHARP
				// Hack - Crestron's Add(object) method doesn't do the necessary cast
				cmd.Parameters.Add(param.InnerObject);
#else
				cmd.Parameters.Add(param);
#endif
			}
		}

		/// <summary>
		/// Gets the column names for the given reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static IEnumerable<string> GetColumnNames(IDataRecord reader)
		{
			return Enumerable.Range(0, reader.FieldCount)
			                 .Select(i => reader.GetName(i))
							 .ToArray();
		}

		/// <summary>
		/// Populate a references type from an IDataRecord by matching column names to property names.
		/// </summary>
		private static void PopulateClass(object objectClass, IDataRecord reader)
		{
			TypeModel typeModel = TypeModel.Get(objectClass.GetType());

			// Only set properties which match column names in the result.
			foreach (string columnName in GetColumnNames(reader))
			{
				object value = reader[columnName];
				if (value != DBNull.Value)
					typeModel.SetPropertyValue(objectClass, columnName, value);
			}
		}

		/// <summary>
		/// If the primary key is a Guid type and the value is default, assign a new Guid.
		/// </summary>
		/// <param name="typeModel"></param>
		/// <param name="paramModel"></param>
		/// <param name="param"></param>
		private static void AutoIncrementGuid(TypeModel typeModel, TypeModel paramModel, object param)
		{
			if (string.IsNullOrEmpty(typeModel.PrimaryKeyName))
				return;

			PropertyInfo property = paramModel.GetProperty(typeModel.PrimaryKeyName);
			if (property.PropertyType != typeof(Guid))
				return;

			Guid value = (Guid)property.GetValue(param, null);
			if (value != default(Guid))
				return;

			property.SetValue(param, Guid.NewGuid(), null);
		}

		/// <summary>
		/// Called immediately after an INSERT to update the given object's primary key with the resulting key in the table.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="tableName"></param>
		/// <param name="typeModel"></param>
		/// <param name="paramModel"></param>
		/// <param name="param"></param>
		private static void QueryLastId(IDbConnection connection, string tableName, TypeModel typeModel, TypeModel paramModel, object param)
		{
			// Does the param have an ID property?
			if (!paramModel.GetPropertyNames().Contains(typeModel.PrimaryKeyName))
				return;

			// Get the ID from the table
			long lastRow = (long)connection.ExecuteScalar("SELECT last_insert_rowid()");
			object id = connection.ExecuteScalar(string.Format("SELECT {0} FROM {1} WHERE _ROWID_={2}", typeModel.PrimaryKeyName, tableName, lastRow));

			// Set the ID on the original param object
			paramModel.SetPropertyValue(param, typeModel.PrimaryKeyName, id);
		}

		#endregion
	}
}
