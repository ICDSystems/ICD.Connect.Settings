using System.Collections.Generic;
using System.Linq;

namespace ICD.Connect.Settings.Core
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
