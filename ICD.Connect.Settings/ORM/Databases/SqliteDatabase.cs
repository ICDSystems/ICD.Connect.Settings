#if SIMPLSHARP
using Crestron.SimplSharp.SQLite;
#else
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
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected override void CreateTable(Type type, string name)
		{
			TypeModel typeModel = TypeModel.Get(type);

			string sql = "CREATE TABLE " + name + " (" + typeModel.GetDelimitedCreateParamList(",") + ")";

			GetConnection().Execute(sql);
		}

		/// <summary>
		/// Throws an exception if the table columns do not match the type properties.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		protected override void ValidateTable(Type type, string name)
		{
			Dictionary<string, SqliteColumnInfo> columns =
				GetConnection().Query<SqliteColumnInfo>(string.Format("PRAGMA table_info({0})", name))
				               .ToDictionary(c => c.name);

			TypeModel model = TypeModel.Get(type);
			string[] propertyNames = model.GetPropertyNames().ToArray();

			// Validate properties
			foreach (string column in columns.Keys)
				if (!propertyNames.Contains(column))
					throw new ApplicationException(string.Format("Type {0} does not have property for table {1} column {2}", type.Name, name, column));

			// Validate columns
			foreach (string propertyName in propertyNames)
				if (!columns.ContainsKey(propertyName))
					throw new ApplicationException(string.Format("Table {0} does not have column for property {1}.{2}", name, type.Name, propertyName));

			// Compare columns
			foreach (string propertyName in propertyNames)
			{
				SqliteColumnInfo columnInfo = columns[propertyName];
				
				// Type
				string expectedType = model.GetPropertySqlType(propertyName);
				if (!expectedType.Equals(columnInfo.type, StringComparison.OrdinalIgnoreCase))
					throw new ApplicationException(string.Format("{0}.{1} does not match SQL type {2} for table {3} column {4}",
					                               type.Name, propertyName, columnInfo.type, name, columnInfo.name));
			}
		}

		/// <summary>
		/// Returns true if a table with the given name exists.
		/// </summary>
		/// <param name="name"></param>
		protected override bool TableExists(string name)
		{
			string sql = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'", name);
			return name == GetConnection().ExecuteScalar(sql) as string;
		}

		private sealed class SqliteColumnInfo
		{
			[DataField] public string cid { get; set; }
			[PrimaryKey] public string name { get; set; }
			[DataField] public string type { get; set; }
			[DataField] public int notnull { get; set; }
			[DataField] public int dflt_value { get; set; }
			[DataField] public int pk { get; set; }
		}
	}
}
