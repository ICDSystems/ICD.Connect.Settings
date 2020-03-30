using System.Collections.Generic;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.SQLite;
#else
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
#endif

namespace ICD.Connect.Settings.ORM.DapperLite
{
	public class SqliteDatabase<TId> : Database<TId>
	{
		public SqliteDatabase(SQLiteConnection connection) : base(connection) { }

		public SqliteDatabase(SQLiteConnection connection, DapperLiteException exceptionHandler, bool throwExceptions)
			: base(connection, exceptionHandler, throwExceptions)
		{
		}

		protected override Dictionary<string, string> GetTableNameMap()
		{
			const string sql = @"
                SELECT tbl_name
                FROM main.sqlite_master
                WHERE type = 'table'";
				
			return GetConnection().Query<string>(sql).ToDictionary(name => name);
		}

		public new SQLiteConnection GetConnection()
		{
			return base.GetConnection() as SQLiteConnection;
		}
	}
}
