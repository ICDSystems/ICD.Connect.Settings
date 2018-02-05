using ICD.Common.Utils.Tests.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Settings.Tests.Attributes.SettingsProperties
{
    public abstract class AbstractSettingsPropertyAttributeTest<TAttribute> : AbstractIcdAttributeTest<TAttribute>
		where TAttribute : AbstractSettingsPropertyAttribute
	{
    }
}
