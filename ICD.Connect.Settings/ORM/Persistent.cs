using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.ORM
{
	public enum eDb
	{
		RoomPreferences,
		RoomData,
		UserPreferences,
		UserData,
		ProgramPreferences,
		ProgramData
	}

	public static class Persistent
	{
		private static readonly Dictionary<DatabasesKey, PersistentDatabase> s_Databases;
		private static readonly SafeCriticalSection s_DatabasesSection;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static Persistent()
		{
			s_Databases = new Dictionary<DatabasesKey, PersistentDatabase>();
			s_DatabasesSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Gets the database for the given category.
		/// </summary>
		/// <param name="category">The type of database</param>
		/// <param name="key">Username, Room ID, etc</param>
		/// <returns></returns>
		public static PersistentDatabase Db(eDb category, string key)
		{
			DatabasesKey cacheKey = new DatabasesKey(category, key);

			return s_DatabasesSection.Execute(() => s_Databases.GetOrAddNew(cacheKey, () => new PersistentDatabase(category, key)));
		}

		private struct DatabasesKey : IEquatable<DatabasesKey>
		{
			private readonly eDb m_Category;
			private readonly string m_Key;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="category"></param>
			/// <param name="key"></param>
			public DatabasesKey(eDb category, string key)
			{
				m_Category = category;
				m_Key = key;
			}

			#region Equality

			/// <summary>
			/// Implementing default equality.
			/// </summary>
			/// <param name="a1"></param>
			/// <param name="a2"></param>
			/// <returns></returns>
			public static bool operator ==(DatabasesKey a1, DatabasesKey a2)
			{
				return a1.Equals(a2);
			}

			/// <summary>
			/// Implementing default inequality.
			/// </summary>
			/// <param name="a1"></param>
			/// <param name="a2"></param>
			/// <returns></returns>
			public static bool operator !=(DatabasesKey a1, DatabasesKey a2)
			{
				return !a1.Equals(a2);
			}

			/// <summary>
			/// Returns true if this instance is equal to the given object.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public override bool Equals(object other)
			{
				return other is DatabasesKey && Equals((DatabasesKey)other);
			}

			/// <summary>
			/// Returns true if this instance is equal to the given endpoint.
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			[Pure]
			public bool Equals(DatabasesKey other)
			{
				return m_Category == other.m_Category &&
					   m_Key == other.m_Key;
			}

			/// <summary>
			/// Gets the hashcode for this instance.
			/// </summary>
			/// <returns></returns>
			[Pure]
			public override int GetHashCode()
			{
				unchecked
				{
					int hash = 17;
					hash = hash * 23 + (int)m_Category;
					hash = hash * 23 + m_Key.GetHashCode();
					return hash;
				}
			}

			#endregion
		}
	}
}