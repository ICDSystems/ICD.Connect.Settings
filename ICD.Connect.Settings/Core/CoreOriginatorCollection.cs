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
