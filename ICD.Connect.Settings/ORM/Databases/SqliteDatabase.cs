using System;
using System.Collections.Generic;
#if SIMPLSHARP
using Crestron.SimplSharp.SQLite;
#else
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
#endif
using ICD.Connect.Settings.ORM.Extensions;

namespace ICD.Connect.Settings.ORM.Databases
{
	public sealed class SqliteDatabase : AbstractDatabase
	{
		public SqliteDatabase(SQLiteConnection connection)
			: base(connection)
		{
		}

		/// <summary>
		/// This abstract method must be implemented by deriving types as
		/// the implementation is specific to the SQL vendor (and possibly version).
		/// </summary>
		protected override IEnumerable<string> GetTables()
		{
			const string sql = @"
                SELECT tbl_name
                FROM main.sqlite_master
                WHERE type = 'table'";

			return GetConnection().Query<string>(sql);
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

			string sql = "CREATE TABLE " + name + " (" + typeModel.GetDelimitedCreateParamList(",")
			             + ", CONSTRAINT PK_" + typeModel.PrimaryKeyName
			             + " PRIMARY KEY(" + typeModel.PrimaryKeyName + ") )";

			GetConnection().Execute(sql);
		}
	}
}
