using System;
using ICD.Connect.Settings.Attributes;
using NUnit.Framework;
#if SIMPLSHARP
using ICD.Common.Utils.Extensions;
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Connect.Settings.Tests.Attributes
{
    [TestFixture]
    public sealed class SettingsPropertyTest
    {
        [TestCase(SettingsProperty.ePropertyType.Hidden)]
        public void PropertyTypeTest(SettingsProperty.ePropertyType expected)
        {
            SettingsProperty property = new SettingsProperty(expected);
            Assert.AreEqual(expected, property.PropertyType);
        }

	    [TestCase(typeof(IOriginator))]
		public void TypeTest(Type expected)
	    {
			SettingsProperty property = new SettingsProperty(SettingsProperty.ePropertyType.Id, expected);
		    Assert.AreEqual(expected, property.Type);
		}

        [Test]
        public void InheritanceTest()
        {
            PropertyInfo property = typeof(B).GetProperty("TestProperty");
            Assert.NotNull(property, "Unable to find property");

            SettingsProperty attribute = property.GetCustomAttribute<SettingsProperty>(true);
            Assert.NotNull(attribute, "Unable to find attribute");

            Assert.AreEqual(SettingsProperty.ePropertyType.Id, attribute.PropertyType, "PropertyType is incorrect");
        }

        private abstract class A
        {
            [SettingsProperty(SettingsProperty.ePropertyType.Id)]
            public int TestProperty { get; set; }
        }

        private sealed class B : A
        {
        }
    }
}
