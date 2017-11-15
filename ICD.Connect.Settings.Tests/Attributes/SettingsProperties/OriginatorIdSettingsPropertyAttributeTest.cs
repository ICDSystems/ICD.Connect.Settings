using System;
using ICD.Connect.Settings.Attributes.SettingsProperties;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Attributes.SettingsProperties
{
	[TestFixture]
    public sealed class OriginatorIdSettingsPropertyAttributeTest : AbstractSettingsPropertyAttributeTest
    {
	    [TestCase(typeof(IOriginator))]
	    public void TypeTest(Type expected)
	    {
		    OriginatorIdSettingsPropertyAttribute property = new OriginatorIdSettingsPropertyAttribute(expected);
		    Assert.AreEqual(expected, property.OriginatorType);
	    }
	}
}
