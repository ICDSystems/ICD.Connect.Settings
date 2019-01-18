using System.Collections.Generic;
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
			: this(Enumerable.Empty<IOriginator>())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="children"></param>
		public CoreOriginatorCollection(IEnumerable<IOriginator> children)
			: base(children)
		{
		}
	}
}
