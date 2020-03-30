using System.Collections.Generic;
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
	}

	public static class Persistent
	{
		private static readonly Dictionary<eDb, PersistentDatabase> s_Databases;
		private static readonly SafeCriticalSection s_DatabasesSection;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static Persistent()
		{
			s_Databases = new Dictionary<eDb, PersistentDatabase>();
			s_DatabasesSection = new SafeCriticalSection();
		}

		/// <summary>
		/// Gets the database for the given category.
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public static PersistentDatabase Database(eDb category)
		{
			return s_DatabasesSection.Execute(() => s_Databases.GetOrAddNew(category, () => new PersistentDatabase(category)));
		}
	}
}