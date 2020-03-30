using System;
using System.Collections.Generic;
using ICD.Common.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
using Crestron.SimplSharp.SQLite;
#else
using System.Data;
using SQLiteConnection = Microsoft.Data.Sqlite.SqliteConnection;
#endif
using ICD.Connect.Settings.ORM.Databases;

namespace ICD.Connect.Settings.ORM
{
	public sealed class PersistentDatabase
	{
		private readonly SqliteDatabase m_Database;

		/// <summary>
		///  constructor.
		/// </summary>
		public PersistentDatabase(eDb category)
		{
			string path = PathUtils.GetProgramDataPath(category + ".sqlite");
			string connectionString = string.Format("Data Source={0};", path);

			SQLiteConnection connection = new SQLiteConnection(connectionString);
			m_Database = new SqliteDatabase(connection);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public int Execute(string sql, object param, IDbTransaction transaction)
		{
			return m_Database.Execute(sql, param, transaction);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public int Execute(string sql)
		{
			return m_Database.Execute(sql);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql, object param)
		{
			return m_Database.Query<T>(sql, param);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql)
		{
			return m_Database.Query<T>(sql);
		}

		/// <summary>
		/// Gets a single instance of a type by specifying the row Id.
		/// </summary>
		/// <returns>A specific instance of the specified type, or the default value for the type.</returns>
		public T Get<T>(Guid id)
		{
			return m_Database.Get<T>(id);
		}

		/// <summary>
		/// Gets a single instance of a type. Filters by a single column.
		/// </summary>
		/// <param name="columnName">Used to generate a WHERE clause.</param>
		/// <param name="data">Input parameter for the WHERE clause.</param>
		/// <returns>A specific instance of the specified type, or the default value for the type.</returns>
		public T Get<T>(string columnName, object data)
		{
			return m_Database.Get<T>(columnName, data);
		}

		/// <summary>
		/// Gets all records in the table matching the supplied type after applying the supplied filter
		/// in a WHERE clause.
		/// </summary>
		/// <param name="columnName">Used to generate a WHERE clause.</param>
		/// <param name="data">Input parameter for the WHERE clause.</param>
		/// <returns>All records in the table matching the supplied type.</returns>
		public IEnumerable<T> All<T>(string columnName, object data)
		{
			return m_Database.All<T>(columnName, data);
		}

		/// <summary>
		/// Gets all records in the table matching the supplied type.
		/// </summary>
		/// <returns>All records in the table matching the supplied type.</returns>
		public IEnumerable<T> All<T>()
		{
			return m_Database.All<T>();
		}

		/// <summary>
		/// Inserts the supplied object into the database. Infers table name from type name.
		/// </summary>
		public void Insert(object obj)
		{
			m_Database.Insert(obj);
		}

		/// <summary>
		/// Updates the supplied object. Infers table name from type name.
		/// </summary>
		public void Update(object obj)
		{
			m_Database.Update(obj);
		}
	}
}
