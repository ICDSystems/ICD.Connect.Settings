using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Migration.Migrators;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Migration.Migrators
{
	public abstract class AbstractConfigVersionMigratorTest
	{
		protected abstract string BeforeConfig { get; }

		protected abstract string AfterConfig { get; }

		[Test]
		public void MigrateTest()
		{
			IConfigVersionMigrator migrator = InstantiateMigrator();
			string result = migrator.Migrate(BeforeConfig);

			AssertXmlEqual(AfterConfig, result);
		}

		protected abstract IConfigVersionMigrator InstantiateMigrator();

		private static void AssertXmlEqual(string expected, string actual)
		{
			if (expected == null && actual == null)
				Assert.Pass("Both strings are null");

			if (expected == null)
				Assert.Fail("Expected null");

			if (actual == null)
				Assert.Fail("Actual string was null");

			expected = XmlUtils.Format(expected);
			actual = XmlUtils.Format(actual);

			Assert.AreEqual(expected, actual);
		}
	}
}
