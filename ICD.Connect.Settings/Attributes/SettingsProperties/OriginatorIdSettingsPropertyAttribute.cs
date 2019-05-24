using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	[MeansImplicitUse]
	public sealed class OriginatorIdSettingsPropertyAttribute : AbstractOriginatorIdSettingsPropertyAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originatorType"></param>
		public OriginatorIdSettingsPropertyAttribute(Type originatorType)
			: base(originatorType)
		{
		}
	}
}
