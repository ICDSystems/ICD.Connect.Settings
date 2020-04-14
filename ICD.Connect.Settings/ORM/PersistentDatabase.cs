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
		/// Gets the first or default item matching the given parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		/// <returns></returns>
		public T Get<T>(object param)
		{
			return m_Database.Get<T>(param);
		}

		/// <summary>
		/// Gets all of the items matching the given parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		/// <returns></returns>
		public IEnumerable<T> All<T>(object param)
		{
			return m_Database.All<T>(param);
		}

		/// <summary>
		/// Gets all records in the table matching the supplied type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>All records in the table matching the supplied type.</returns>
		public IEnumerable<T> All<T>()
		{
			return m_Database.All<T>();
		}

		/// <summary>
		/// Inserts the supplied object into the database. Infers table name from type name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		public void Insert<T>(object param)
		{
			using (IDbTransaction transaction = m_Database.GetConnection().BeginTransaction())
			{
				m_Database.Insert<T>(transaction, param);
				transaction.Commit();
			}
		}

		/// <summary>
		/// Updates the supplied object. Infers table name from type name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		public void Update<T>(object param)
		{
			using (IDbTransaction transaction = m_Database.GetConnection().BeginTransaction())
			{
				m_Database.Update<T>(transaction, param);
				transaction.Commit();
			}
		}

		/// <summary>
		/// Deletes the matching objects from the table.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		public void Delete<T>(object param)
		{
			using (IDbTransaction transaction = m_Database.GetConnection().BeginTransaction())
			{
				m_Database.Delete<T>(transaction, param);
				transaction.Commit();
			}
		}
	}
}
