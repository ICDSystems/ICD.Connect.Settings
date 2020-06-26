#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
using Crestron.SimplSharp.SQLite;
#else
using System.Data;
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Connect.Settings.ORM.Extensions;

namespace ICD.Connect.Settings.ORM.Databases
{
	public sealed class SqliteDatabase : AbstractDatabase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connection"></param>
		public SqliteDatabase(SQLiteConnection connection)
			: base(connection)
		{
		}

		/// <summary>
		/// Creates the table for the given type.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected override void CreateTable(IDbTransaction transaction, Type type, string name)
		{
			TypeModel typeModel = TypeModel.Get(type);

			string sql = "CREATE TABLE " + name + " (" + typeModel.GetDelimitedCreateParamList(",") + ")";

			GetConnection().Execute(sql, null, transaction);
		}

		/// <summary>
		/// Throws an exception if the table columns do not match the type properties.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		protected override void ValidateTable(Type type, string name)
		{
			TypeModel model = TypeModel.Get(type);

			Dictionary<string, SqliteColumnInfo> tableColumns =
				Query<SqliteColumnInfo>(string.Format("PRAGMA table_info({0})", name))
					.ToDictionary(c => c.name);

			Dictionary<string, PropertyModel> modelColumns =
				model.GetColumns()
				     .ToDictionary(p => p.Name);

			// Validate properties
			foreach (string tableColumn in tableColumns.Keys)
				if (!modelColumns.ContainsKey(tableColumn))
					throw new ApplicationException(string.Format("Type {0} does not have property for table {1} column {2}", type.Name, name, tableColumn));

			// Validate columns
			foreach (string modelColumn in modelColumns.Keys)
				if (!tableColumns.ContainsKey(modelColumn))
					throw new ApplicationException(string.Format("Table {0} does not have column for property {1}.{2}", name, type.Name, modelColumn));

			// Compare columns
			foreach (PropertyModel modelColumn in modelColumns.Values)
			{
				SqliteColumnInfo columnInfo = tableColumns[modelColumn.Name];
				
				// Type
				if (!modelColumn.SqlType.Equals(columnInfo.type, StringComparison.OrdinalIgnoreCase))
					throw new ApplicationException(string.Format("{0}.{1} does not match SQL type {2} for table {3} column {4}",
					                               type.Name, modelColumn, columnInfo.type, name, columnInfo.name));
			}
		}

		/// <summary>
		/// Returns true if a table with the given name exists.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="name"></param>
		protected override bool TableExists(IDbTransaction transaction, string name)
		{
			string sql = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'", name);
			return name == GetConnection().ExecuteScalar(sql, null, transaction) as string;
		}

		// public due to Eazfuscator issue related to properties on a nested private class
		public sealed class SqliteColumnInfo
		{
			// ReSharper disable InconsistentNaming
			[DataField] public string cid { get; set; }
			[PrimaryKey] public string name { get; set; }
			[DataField] public string type { get; set; }
			[DataField] public int notnull { get; set; }
			[DataField] public object dflt_value { get; set; }
			[DataField] public int pk { get; set; }
			// ReSharper restore InconsistentNaming
		}
	}
}
