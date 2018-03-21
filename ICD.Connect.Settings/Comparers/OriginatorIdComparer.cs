using System.Collections.Generic;
using ICD.Common.Utils.Comparers;

namespace ICD.Connect.Settings.Comparers
{
	public abstract class AbstractOriginatorIdComparer<T> : IComparer<T>
		where T : IOriginator
	{
		private readonly PropertyComparer<T, int> m_IdComparer;

		protected AbstractOriginatorIdComparer()
		{
			m_IdComparer = new PropertyComparer<T, int>(Comparer<int>.Default, o => o.Id);
		}

		public int Compare(T x, T y)
		{
			return m_IdComparer.Compare(x, y);
		}
	}

	public sealed class OriginatorIdComparer : AbstractOriginatorIdComparer<IOriginator>
	{
	}

	public sealed class OriginatorIdComparer<T> : AbstractOriginatorIdComparer<T>
		where T : IOriginator
	{
	}
}
