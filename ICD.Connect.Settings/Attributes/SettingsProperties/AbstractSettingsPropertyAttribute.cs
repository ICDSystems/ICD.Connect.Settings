using System;
using ICD.Common.Utils.Attributes;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class AbstractSettingsPropertyAttribute : AbstractIcdAttribute, ISettingsPropertyAttribute
	{
	}
}
