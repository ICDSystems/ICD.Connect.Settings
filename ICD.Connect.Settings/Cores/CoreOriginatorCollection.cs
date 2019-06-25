using System.Linq;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Cores
{
	public sealed class CoreOriginatorCollection : AbstractOriginatorCollection<IOriginator>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public CoreOriginatorCollection()
			: base(Enumerable.Empty<IOriginator>())
		{

		}
	}
}
