using System.Collections.Generic;

namespace ICD.Connect.Settings.Core
{
	public class CoreOriginatorCollection : AbstractOriginatorCollection<IOriginator>
	{
		public CoreOriginatorCollection()
		{
			
		}

		public CoreOriginatorCollection(IEnumerable<IOriginator> children)
			: base(children)
		{
			
		}
	}
}