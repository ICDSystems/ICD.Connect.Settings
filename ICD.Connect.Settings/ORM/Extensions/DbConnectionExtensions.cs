using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
#else
using System.Data;
#endif

namespace ICD.Connect.Settings.ORM.Extensions
{
	public static class DbConnectionExtensions
	{
		#region Methods

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

		/// <summary>
		/// Overload to allow a basic execute call.
		/// </summary>
		/// <returns>Number of rows affected.</returns>
		public static int Execute(this IDbConnection cnn, string sql)
		{
			return Execute(cnn, sql, null, null);
		}

		/// <summary>
		/// Basically a duplication of the Dapper interface.
		/// </summary>
		public static IEnumerable<T> Query<T>(this IDbConnection conn, string sql, object param)
		{
			IList<T> list = new List<T>();

			using (IDbCommand cmd = SetupCommand(conn, null, sql, null, null))
			{
				AddParams(cmd, param);

				using (IDataReader reader = cmd.ExecuteReader())
				{
					if (reader == null)
						return list;

					Type type = typeof(T);
					if (type.IsValueType || type == typeof(string))
					{
						while (reader.Read())
						{
							if (reader.IsDBNull(0)) // Handles the case where the value is null.
							{
								list.Add(default(T));
							}
							else
							{
								list.Add((T)reader[0]);
							}
						}
					}
					else // Reference types
					{
						while (reader.Read())
						{
							T record = ReflectionUtils.CreateInstance<T>();
							PopulateClass(record, reader);
							list.Add(record);
						}
					}

					return list;
				}
			}
		}

		/// <summary>
		/// Overload for the times when we don't require a parameter.
		/// Because of .NET Compact Framework, we can't use optional parameters.
		/// </summary>
		public static IEnumerable<T> Query<T>(this IDbConnection conn, string sql)
		{
			return Query<T>(conn, sql, null);
		}

		/// <summary>
		/// Inspired by Dapper.Rainbow.
		/// </summary>
		public static void Insert(this IDbConnection connection, IDbTransaction transaction, string tableName, object param)
		{
			TypeModel paramModel = TypeModel.Get(param.GetType());

			string cols = paramModel.GetDelimitedSafeFieldList(",");
			string colsParams = paramModel.GetDelimitedSafeParamList(",");
			string sql = "INSERT INTO " + tableName + " (" + cols + ") VALUES (" + colsParams + ")";

			int result = connection.Execute(sql, param, transaction);

			if (result <= 0)
				throw new ApplicationException("Return value of INSERT should be greater than 0. An error has occurred with the INSERT.");
		}

		/// <summary>
		/// Inspired by Dapper.Rainbow.
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
				throw new ApplicationException("Return value of UPDATE should be greater than 0. An error has occurred with the INSERT.");
		}

		#endregion

		#region Private Methods

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
					param.DbType = typeModel.GetPropertyType(data, propertyName);
					param.Value = typeModel.GetPropertyValue(data, propertyName) ?? DBNull.Value;
				}
				cmd.Parameters.Add(param);
			}
		}

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
				if (value == DBNull.Value)
					continue;

				typeModel.SetPropertyValue(objectClass, columnName, value);
			}
		}

		#endregion
	}
}
