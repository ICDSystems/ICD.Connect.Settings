using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
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
		/// Overload to allow a basic execute call.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sql"></param>
		/// <returns>Number of rows affected.</returns>
		public static int Execute(this IDbConnection extends, string sql)
		{
			return extends.Execute(sql, null, null);
		}

		/// <summary>
		/// Executes an SQL statement and returns the number of rows affected. Supports transactions.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns>Number of rows affected.</returns>
		public static int Execute(this IDbConnection extends, string sql, object param, IDbTransaction transaction)
		{
			using (IDbCommand cmd = SetupCommand(extends, transaction, sql, null, null))
			{
				AddParams(cmd, param);
				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Overload to allow a basic execute scalar call.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sql"></param>
		/// <returns>Number of rows affected.</returns>
		public static object ExecuteScalar(this IDbConnection extends, string sql)
		{
			return extends.ExecuteScalar(sql, null, null);
		}

		/// <summary>
		/// Executes an SQL statement and returns the result of the execution. Supports transactions.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sql"></param>
		/// <param name="param"></param>
		/// <param name="transaction"></param>
		/// <returns>Number of rows affected.</returns>
		public static object ExecuteScalar(this IDbConnection extends, string sql, object param, IDbTransaction transaction)
		{
			using (IDbCommand cmd = SetupCommand(extends, transaction, sql, null, null))
			{
				AddParams(cmd, param);
				return cmd.ExecuteScalar();
			}
		}

		/// <summary>
		/// Returns all of the rows for the given SQL query.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sql"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<object> Query(this IDbConnection extends, string sql, Type type)
		{
			return extends.Query(sql, type, new {});
		}

		/// <summary>
		/// Returns all of the rows for the given SQL query, injecting the given parameters into the command.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="sql"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public static IEnumerable<object> Query(this IDbConnection extends, string sql, Type type, object param)
		{
			using (IDbCommand cmd = SetupCommand(extends, null, sql, null, null))
			{
				AddParams(cmd, param);

				using (IDataReader reader = cmd.ExecuteReader())
				{
					if (reader == null)
						return Enumerable.Empty<object>();

					if (type.IsValueType || type == typeof(string))
						return ReadValues(reader, type).ToArray();

					return ReadReferences(reader, type).ToArray();
				}
			}
		}

		/// <summary>
		/// Returns all records in the given table.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<object> All(this IDbConnection extends, Type type)
		{
			return extends.All(type, new {});
		}

		/// <summary>
		/// Returns all of the records in the given table matching the given parameters.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		/// <returns></returns>
		public static IEnumerable<object> All(this IDbConnection extends, Type type, object param)
		{
			TypeModel typeModel = TypeModel.Get(type);
			TypeModel paramModel = TypeModel.Get(param.GetType());

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("SELECT ");
				builder.Append(typeModel.GetDelimitedColumnNames(","));
				builder.Append(" FROM ");
				builder.Append(typeModel.TableName);

				string[] properties = paramModel.GetColumns().Select(p => p.Name).ToArray();
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

			return extends.Query(sql, type, param);
		}

		/// <summary>
		/// If the param primary key is found in the table the row is updated.
		/// Otherwise the param is inserted into the table.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		public static void Upsert(this IDbConnection extends, IDbTransaction transaction, Type type, object param)
		{
			TypeModel typeModel = TypeModel.Get(type);
			PropertyModel primaryKey = typeModel.PrimaryKey;

			string sql = string.Format("SELECT 1 FROM {0} WHERE {1}=@Pk", typeModel.TableName, primaryKey.Name);

			if (extends.Execute(sql, new {Pk = primaryKey.GetDatabaseValue(param)}, transaction) == 1)
				extends.Update(transaction, type, param);
			else
				extends.Insert(transaction, type, param);
		}

		/// <summary>
		/// Inserts the given parameters into the given table.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		public static void Insert(this IDbConnection extends, IDbTransaction transaction, Type type, object param)
		{
			TypeModel typeModel = TypeModel.Get(type);
			TypeModel paramModel = TypeModel.Get(param.GetType());

			string[] columns =
				paramModel.GetColumns()
					// Don't try to insert the primary key if it auto-increments
				          .Where(p => !(typeModel.PrimaryKey != null &&
				                        p.Name == typeModel.PrimaryKey.Name &&
				                        typeModel.PrimaryKey.AutoIncrements))
				          .Select(p => p.Name)
				          .ToArray();

			// "auto-increment" behaviour for Guid ids
			AutoIncrementGuid(typeModel, paramModel, param);

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("INSERT INTO ").Append(typeModel.TableName);

				// Columns
				builder.Append("(");
				builder.Append(string.Join(",", columns));
				builder.Append(")");

				builder.Append(" VALUES ");

				// Values
				builder.Append("(");
				builder.Append(string.Join(",", columns.Select(c => string.Format("@{0}", c)).ToArray()));
				builder.Append(")");
			}
			string sql = builder.ToString();

			int result = extends.Execute(sql, param, transaction);
			if (result <= 0)
				throw new ApplicationException("Return value of INSERT should be greater than 0. An error has occurred with the INSERT.");

			// Assign the inserted ID back onto the param
			QueryLastId(extends, typeModel, paramModel, param);

			// Insert/Update/Delete child items
			UpdateChildItems(extends, transaction, type, param);
		}

		/// <summary>
		/// Updates the record matching the given primary key with the given parameters.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		public static void Update(this IDbConnection extends, IDbTransaction transaction, Type type, object param)
		{
			TypeModel typeModel = TypeModel.Get(type);
			TypeModel paramModel = TypeModel.Get(param.GetType());

			if (typeModel.PrimaryKey == null)
				throw new InvalidOperationException(string.Format("{0} has no primary key", type.Name));

			string[] columns =
				paramModel.GetColumns()
					// Don't try to update the primary key
				          .Where(p => p.Name != typeModel.PrimaryKey.Name)
				          .Select(p => p.Name)
				          .ToArray();

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("UPDATE ").Append(typeModel.TableName).Append(" SET ");
				builder.Append(string.Join(",", columns.Select(c => string.Format("{0}=@{0}", c)).ToArray()));
				builder.Append(string.Format(" WHERE {0}=@{0}", typeModel.PrimaryKey.Name));
			}
			string sql = builder.ToString();

			int result = extends.Execute(sql, param, transaction);
			if (result <= 0)
				throw new ApplicationException("Return value of UPDATE should be greater than 0. An error has occurred with the UPDATE.");

			// Insert/Update child items
			UpdateChildItems(extends, transaction, type, param);
		}

		/// <summary>
		/// Deletes the records matching the given parameters.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		public static void Delete(this IDbConnection extends, IDbTransaction transaction, Type type, object param)
		{
			TypeModel typeModel = TypeModel.Get(type);
			TypeModel paramModel = TypeModel.Get(param.GetType());

			StringBuilder builder = new StringBuilder();
			{
				builder.Append("DELETE FROM ").Append(typeModel.TableName);

				string[] columns = paramModel.GetColumns().Select(p => p.Name).ToArray();
				if (columns.Length != 0)
					builder.Append(" WHERE ");

				for (int index = 0; index < columns.Length; index++)
				{
					if (index != 0)
						builder.Append(" AND ");

					string property = columns[index];
					builder.AppendFormat("{0}=@{0}", property);
				}
			}
			string sql = builder.ToString();

			int result = extends.Execute(sql, param, transaction);
			if (result < 0)
				throw new ApplicationException("Return value of DELETE should be greater than or equal to 0. An error has occurred with the DELETE.");
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reads the rows in the reader as the given value type.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<object> ReadValues(IDataReader reader, Type type)
		{
			while (reader.Read())
			{
				// Handles the case where the value is null.
				if (reader.IsDBNull(0))
					yield return ReflectionUtils.CreateInstance(type);
				else
					yield return reader[0];
			}
		}

		/// <summary>
		/// Reads the rows in the reader as the given reference type.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<object> ReadReferences(IDataReader reader, Type type)
		{
			while (reader.Read())
			{
				object record = ReflectionUtils.CreateInstance(type);
				PopulateInstance(record, reader);
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

			foreach (PropertyModel column in typeModel.GetColumns())
			{
				IDbDataParameter param = cmd.CreateParameter();
				{
					param.ParameterName = "@" + column.Name;
					//param.DbType = column.DbType;
					param.Value = column.GetDatabaseValue(data);
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
		private static void PopulateInstance(object instance, IDataRecord reader)
		{
			TypeModel typeModel = TypeModel.Get(instance.GetType());

			// Populate the column properties
			foreach (string columnName in GetColumnNames(reader))
			{
				object value = reader[columnName];
				typeModel.GetProperty(columnName).SetDatabaseValue(instance, value);
			}

			// TODO - Populate the foreign keys
		}

		/// <summary>
		/// If the primary key is a Guid type and the value is default, assign a new Guid.
		/// </summary>
		/// <param name="typeModel"></param>
		/// <param name="paramModel"></param>
		/// <param name="param"></param>
		private static void AutoIncrementGuid(TypeModel typeModel, TypeModel paramModel, object param)
		{
			if (typeModel.PrimaryKey == null)
				return;

			PropertyModel propertyModel = paramModel.GetProperty(typeModel.PrimaryKey.Name);
			if (propertyModel.PropertyType != typeof(Guid))
				return;

			Guid value = (Guid)propertyModel.Property.GetValue(param, null);
			if (value != default(Guid))
				return;

			propertyModel.Property.SetValue(param, Guid.NewGuid(), null);
		}

		/// <summary>
		/// Called immediately after an INSERT to update the given object's primary key with the resulting key in the table.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="typeModel"></param>
		/// <param name="paramModel"></param>
		/// <param name="param"></param>
		private static void QueryLastId(IDbConnection connection, TypeModel typeModel, TypeModel paramModel, object param)
		{
			string primaryKeyName = typeModel.PrimaryKey == null ? null : typeModel.PrimaryKey.Name;
			PropertyModel primaryKey = paramModel.GetColumns().FirstOrDefault(c => c.Name == primaryKeyName);

			// Does the param have an ID property?
			if (primaryKey == null)
				return;

			// Get the ID from the table
			long lastRow = (long)connection.ExecuteScalar("SELECT last_insert_rowid()");
			object id = connection.ExecuteScalar(string.Format("SELECT {0} FROM {1} WHERE _ROWID_={2}", primaryKeyName, typeModel.TableName, lastRow));

			// Set the ID on the original param object
			primaryKey.SetDatabaseValue(param, id);
		}

		/// <summary>
		/// Finds the child foreign items for the given param and inserts/updates them.
		/// If a foreign item is null, or missing from a collection, the item is deleted.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		private static void UpdateChildItems(IDbConnection connection, IDbTransaction transaction, Type type, object param)
		{
			TypeModel typeModel = TypeModel.Get(type);
			TypeModel paramModel = TypeModel.Get(param.GetType());

			// Get the properties that point to foreign children
			IEnumerable<PropertyModel> childProperties =
				paramModel.GetProperties()
				.Where(p =>
				       {
					       PropertyModel typeModelProperty = typeModel.GetProperty(p.Name);
					       return !typeModelProperty.IsColumn &&
					              typeModelProperty.IsForeignKey;
				       }
					);

			// TODO - Treat properties missing from the parameter as deletions?

			foreach (PropertyModel property in childProperties)
				UpdateChildItems(connection, transaction, type, param, property);
		}

		/// <summary>
		/// Finds the child foreign items for the given param property and inserts/updates them.
		/// If a foreign item is null, or missing from a collection, the item is deleted.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		/// <param name="paramProperty"></param>
		private static void UpdateChildItems(IDbConnection connection, IDbTransaction transaction, Type type,
		                                     object param, PropertyModel paramProperty)
		{
			TypeModel typeModel = TypeModel.Get(type);
			PropertyModel typeModelProperty = typeModel.GetProperty(paramProperty.Name);
			Type childType = typeModelProperty.PropertyOrEnumerableType;

			if (typeModelProperty.IsEnumerable)
			{
				IEnumerable value = paramProperty.Property.GetValue(param, null) as IEnumerable;
				IEnumerable<object> toUpdate = value == null ? null : value.Cast<object>();
				UpdateChildItemsSequence(connection, transaction, type, param, toUpdate, childType);
			}
			else
			{
				object toUpdate = paramProperty.GetDatabaseValue(param);
				UpdateChildItemSingle(connection, transaction, type, param, toUpdate, childType);
			}
		}

		/// <summary>
		/// Inserts/updates the child items.
		/// If a foreign item is missing from the sequence the item is deleted.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		/// <param name="childItems"></param>
		/// <param name="childType"></param>
		private static void UpdateChildItemsSequence(IDbConnection connection, IDbTransaction transaction, Type type,
		                                             object param, IEnumerable<object> childItems, Type childType)
		{
			foreach (object item in childItems)
			{
				if (item == null)
					throw new ArgumentException("Sequence contains a null item");

				UpdateChildItemSingle(connection, transaction, type, param, item, childType);
			}

			// TODO - Delete any child items not in the sequence
		}

		/// <summary>
		/// Inserts/updates the given child item of the given type.
		/// If the child item is null, deletes the foreign item for the parent type.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="param"></param>
		/// <param name="childItem"></param>
		/// <param name="childType"></param>
		private static void UpdateChildItemSingle(IDbConnection connection, IDbTransaction transaction, Type type,
		                                          object param, object childItem, Type childType)
		{
			TypeModel typeModel = TypeModel.Get(type);
			TypeModel childTypeModel = TypeModel.Get(childType);
			PropertyModel foreignPropertyToParent = childTypeModel.GetColumns().Single(p => p.ForeignKeyType == type);
			object parentId = typeModel.PrimaryKey.GetDatabaseValue(param);

			// If the item is null, delete any existing foreign item
			if (childItem == null)
			{
				// Trying to use the existing delete method gets clunky
				string sql = string.Format("DELETE FROM {0} WHERE {1}=@ParentId", childTypeModel.TableName,
				                           foreignPropertyToParent.Name);
				connection.Execute(sql, new {ParentId = parentId}, transaction);
				return;
			}

			// Set the foreign key to the parent ID
			foreignPropertyToParent.SetDatabaseValue(childItem, parentId);

			// Insert/update the child item
			connection.Upsert(transaction, childType, childItem);
		}

		#endregion
	}
}
