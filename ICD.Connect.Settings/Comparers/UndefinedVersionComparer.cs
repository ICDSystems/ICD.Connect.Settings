using System;
using System.Collections.Generic;

namespace ICD.Connect.Settings.Comparers
{
	/// <summary>
	/// Undefined Versions have a value of 0.0.-1.-1
	/// This comparer Maxs Versions to 0.0.0.0
	/// </summary>
	public sealed class UndefinedVersionComparer : IComparer<Version>
	{
		private static UndefinedVersionComparer s_Instance;

		public static UndefinedVersionComparer Instance
		{
			get { return s_Instance = s_Instance ?? new UndefinedVersionComparer(); }
		}

		public int Compare(Version x, Version y)
		{
			int result = Compare(x.Major, y.Major);
			if (result != 0)
				return result;

			result = Compare(x.Minor, y.Minor);
			if (result != 0)
				return result;

			result = Compare(x.Build, y.Build);
			if (result != 0)
				return result;

			return Compare(x.Revision, y.Revision);
		}

		private static int Compare(int x, int y)
		{
			x = x > 0 ? x : 0;
			y = y > 0 ? y : 0;

			return x.CompareTo(y);
		}
	}
}
