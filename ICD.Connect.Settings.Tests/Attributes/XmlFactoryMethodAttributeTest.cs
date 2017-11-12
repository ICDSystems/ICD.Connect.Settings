using ICD.Common.Utils.Tests.Attributes;
using ICD.Connect.Settings.Attributes;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Attributes
{
	[TestFixture]
    public sealed class XmlFactoryMethodAttributeTest : AbstractIcdAttributeTest
    {
		[TestCase("Test")]
		public void FactoryNameTest(string name)
		{
			Assert.AreEqual(name, new XmlFactoryMethodAttribute(name).FactoryName);
		}
    }
}
