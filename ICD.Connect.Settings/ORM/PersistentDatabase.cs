using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;
#if !NETSTANDARD
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
		/// Constructor.
		/// </summary>
		public PersistentDatabase(eDb category, string key)
		{
			string path = BuildPath(category, key);
			if (!IcdFile.Exists(path))
			{
				string directory = IcdPath.GetDirectoryName(path);
				IcdDirectory.CreateDirectory(directory);

				using (IcdFileStream fs = IcdFile.Create(path))
					fs.Close();
			}

			string connectionString = string.Format("Data Source={0};", path);

			SQLiteConnection connection = new SQLiteConnection(connectionString);
			m_Database = new SqliteDatabase(connection);
		}

		#region Methods

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql)
		{
			return m_Database.Query<T>(sql);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql, object param)
		{
			return m_Database.Query<T>(sql, param);
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

		#endregion

		#region Private Methods

		/// <summary>
		/// Builds a path to the .sqlite file for the given category and key.
		///
		///		E.g.
		///			eDb.RoomPreferences for room ID "1001"
		///			returns "../USER/ProgramXXData/Room1001Data/RoomPreferences.sqlite"
		/// 
		///			eDb.UserData for user named "Chris Cameron"
		///			returns "../USER/ProgramXXData/UserChrisCameronData/UserData.sqlite"
		///
		///			eDb.ProgramData
		///			returns "../USER/ProgramXXData/ProgramData.sqlite"
		///
		/// </summary>
		/// <param name="category"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private static string BuildPath(eDb category, string key)
		{
			switch (category)
			{
				case eDb.RoomPreferences:
				case eDb.RoomData:
					return PathUtils.GetRoomDataPath(int.Parse(key), category + ".sqlite");

				case eDb.UserPreferences:
				case eDb.UserData:
					return PathUtils.GetUserDataPath(key, category + ".sqlite");

				case eDb.ProgramPreferences:
				case eDb.ProgramData:
					if (key != null)
						throw new ArgumentException("ProgramData does not take a key", key);
					return PathUtils.GetProgramDataPath(category + ".sqlite");

				default:
					throw new ArgumentOutOfRangeException("category");
			}
		}

		#endregion
	}
}
