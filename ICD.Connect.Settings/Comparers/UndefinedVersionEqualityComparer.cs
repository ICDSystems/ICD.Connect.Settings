using System;
using System.Collections.Generic;

namespace ICD.Connect.Settings.Comparers
{
	/// <summary>
	/// Undefined Versions have a value of 0.0.-1.-1
	/// This comparer Maxs Versions to 0.0.0.0
	/// </summary>
	public sealed class UndefinedVersionEqualityComparer : IEqualityComparer<Version>
	{
		private static UndefinedVersionEqualityComparer s_Instance;

		public static UndefinedVersionEqualityComparer Instance
		{
			get { return s_Instance = s_Instance ?? new UndefinedVersionEqualityComparer(); }
		}

		public bool Equals(Version x, Version y)
		{
			return Equals(x.Major, y.Major) &&
			       Equals(x.Minor, y.Minor) &&
			       Equals(x.Build, y.Build) &&
			       Equals(x.Revision, y.Revision);
		}

		private static bool Equals(int x, int y)
		{
			x = x > 0 ? x : 0;
			y = y > 0 ? y : 0;

			return x.Equals(y);
		}

		public int GetHashCode(Version version)
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + GetHashCode(version.Major);
				hash = hash * 23 + GetHashCode(version.Minor);
				hash = hash * 23 + GetHashCode(version.Build);
				hash = hash * 23 + GetHashCode(version.Revision);
				return hash;
			}
		}

		private static int GetHashCode(int value)
		{
			return (value > 0 ? value : 0).GetHashCode();
		}
	}
}