using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
#else
using System.Data;
#endif
using ICD.Connect.Settings.ORM.Extensions;

namespace ICD.Connect.Settings.ORM.Databases
{
	/// <summary>
	/// A container for a database. Assumes all tables have an Id column named Id.
	/// </summary>
	public abstract class AbstractDatabase
	{
		private readonly IDbConnection m_Connection;
		private readonly IcdHashSet<string> m_TableNames;

		/// <summary>
		/// Provides advanced configuration for the behaviour of the class when Exceptions are encountered.
		/// </summary>
		/// <param name="connection"></param>
		protected AbstractDatabase([NotNull] IDbConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			m_Connection = connection;
			m_TableNames = new IcdHashSet<string>();
		}

		#region Methods

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public int Execute(string sql, object param, IDbTransaction transaction)
		{
			return GetConnection().Execute(sql, param, transaction);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public int Execute(string sql)
		{
			return Execute(sql, null, null);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql, object param)
		{
			return GetConnection().Query<T>(sql, param);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql)
		{
			return Query<T>(sql, null);
		}

		/// <summary>
		/// Gets a single instance of a type. Filters by the given parameters.
		/// </summary>
		/// <param name="param"></param>
		/// <returns>A specific instance of the specified type, or the default value for the type.</returns>
		public T Get<T>(object param)
		{
			return All<T>(param).FirstOrDefault();
		}

		/// <summary>
		/// Gets all records in the table matching the given parameters.
		/// </summary>
		/// <returns>All records in the table matching the supplied type.</returns>
		public IEnumerable<T> All<T>(object param)
		{
			string tableName = LazyLoadTable(typeof(T));
			return GetConnection().All<T>(tableName, param);
		}

		/// <summary>
		/// Gets all records in the table matching the supplied type.
		/// </summary>
		/// <returns>All records in the table matching the supplied type.</returns>
		public IEnumerable<T> All<T>()
		{
			string tableName = LazyLoadTable(typeof(T));
			return GetConnection().All<T>(tableName);
		}

		/// <summary>
		/// Inserts the supplied object into the database. Infers table name from type name.
		/// </summary>
		public void Insert<T>(object param)
		{
			string tableName = LazyLoadTable(typeof(T));
			GetConnection().Insert<T>(null, tableName, param);
		}

		/// <summary>
		/// Updates the supplied object. Infers table name from type name.
		/// </summary>
		public void Update<T>(object param)
		{
			string tableName = LazyLoadTable(typeof(T));
			GetConnection().Update<T>(null, tableName, param);
		}

		/// <summary>
		/// Deletes the objects in the database matching the params.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="param"></param>
		public void Delete<T>(object param)
		{
			string tableName = LazyLoadTable(typeof(T));
			GetConnection().Delete<T>(null, tableName, param);
		}

		#endregion

		#region Protected Methods

		protected IDbConnection GetConnection()
		{
			if (m_Connection.State != ConnectionState.Open)
				m_Connection.Open();

			return m_Connection;
		}

		/// <summary>
		/// This abstract method must be implemented by deriving types as
		/// the implementation is specific to the SQL vendor (and possibly version).
		/// </summary>
		protected abstract IEnumerable<string> GetTables();

		/// <summary>
		/// Creates the table for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected abstract void CreateTable(Type type, string name);

		#endregion

		#region Private Methods

		private string LazyLoadTable(Type type)
		{
			if (m_TableNames.Count == 0)
				m_TableNames.AddRange(GetTables());

			string tableName = TypeModel.Get(type).TableName;

			if (!m_TableNames.Contains(tableName))
				CreateTable(type, tableName);
			m_TableNames.Add(tableName);

			return tableName;
		}

		#endregion
	}
}
