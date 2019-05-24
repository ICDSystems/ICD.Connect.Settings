using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Attributes;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	[AttributeUsage(AttributeTargets.Property)]
	[MeansImplicitUse]
	public abstract class AbstractSettingsPropertyAttribute : AbstractIcdAttribute, ISettingsPropertyAttribute
	{
	}
}
