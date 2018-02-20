using System;

namespace ICD.Connect.Settings
{
	public sealed class NullSettings : AbstractSettings
	{
		protected override string Element { get { return null; } }

		public override string FactoryName { get { return null; } }

		public override Type OriginatorType { get { return null; } }
	}
}
