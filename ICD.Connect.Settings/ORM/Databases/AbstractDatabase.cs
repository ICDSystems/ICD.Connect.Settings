using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;
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
		public IEnumerable<T> Query<T>(string sql)
		{
			return Query<T>(sql, null);
		}

		/// <summary>
		/// A wrapper that uses the internally cached connection.
		/// </summary>
		public IEnumerable<T> Query<T>(string sql, object param)
		{
			LazyLoadTable(null, typeof(T));
			return GetConnection().Query(sql, typeof(T), param).Cast<T>();
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
			LazyLoadTable(null, typeof(T));
			return GetConnection().All(typeof(T), param).Cast<T>();
		}

		/// <summary>
		/// Gets all records in the table matching the supplied type.
		/// </summary>
		/// <returns>All records in the table matching the supplied type.</returns>
		public IEnumerable<T> All<T>()
		{
			LazyLoadTable(null, typeof(T));
			return GetConnection().All(typeof(T)).Cast<T>();
		}

		/// <summary>
		/// Inserts the supplied object into the database. Infers table name from type name.
		/// </summary>
		public void Insert<T>(IDbTransaction transaction, object param)
		{
			LazyLoadTable(transaction, typeof(T));
			GetConnection().Insert(transaction, typeof(T), param);
		}

		/// <summary>
		/// Updates the supplied object. Infers table name from type name.
		/// </summary>
		public void Update<T>(IDbTransaction transaction, object param)
		{
			LazyLoadTable(transaction, typeof(T));
			GetConnection().Update(transaction, typeof(T), param);
		}

		/// <summary>
		/// Deletes the objects in the database matching the params.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="transaction"></param>
		/// <param name="param"></param>
		public void Delete<T>(IDbTransaction transaction, object param)
		{
			LazyLoadTable(transaction, typeof(T));
			GetConnection().Delete(transaction, typeof(T), param);
		}

		#endregion

		#region Protected Methods

		public IDbConnection GetConnection()
		{
			if (m_Connection.State != ConnectionState.Open)
				m_Connection.Open();

			return m_Connection;
		}

		/// <summary>
		/// Creates the table for the given type.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		protected abstract void CreateTable(IDbTransaction transaction, Type type, string name);

		/// <summary>
		/// Throws an exception if the table columns do not match the type properties.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		protected abstract void ValidateTable(Type type, string name);

		/// <summary>
		/// Returns true if a table with the given name exists.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="name"></param>
		protected abstract bool TableExists(IDbTransaction transaction, string name);

		/// <summary>
		/// Checks to see if a table exists for the given type.
		/// Creates the table and tables for foreign objects recursively if not.
		/// Performs validation to ensure the tables match the given Type.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		protected virtual void LazyLoadTable(IDbTransaction transaction, Type type)
		{
			string tableName = TypeModel.Get(type).TableName;

			if (!m_TableNames.Contains(tableName))
			{
				if (!TableExists(transaction, tableName))
					CreateTable(transaction, type, tableName);

				ValidateTable(type, tableName);

				LazyLoadForeignTables(transaction, type);

				m_TableNames.Add(tableName);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Loops over the foreign children in the Type and creates tables recursively.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="type"></param>
		private void LazyLoadForeignTables(IDbTransaction transaction, Type type)
		{
			TypeModel.Get(type)
			         .GetProperties()
			         .Where(p => !p.IsColumn && p.IsForeignKey)
			         .Select(p => p.PropertyOrEnumerableType)
			         .ForEach(t => LazyLoadTable(transaction, t));
		}

		#endregion
	}
}
