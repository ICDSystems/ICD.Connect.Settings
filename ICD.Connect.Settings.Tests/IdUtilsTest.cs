using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Connect.Settings.Tests
{
	[TestFixture]
    public sealed class IdUtilsTest
    {
		[Test]
		public void GetNewIdTest()
		{
			IEnumerable<int> ids = Enumerable.Range(1, 10);
			Assert.AreEqual(11, IdUtils.GetNewId(ids));
		}
	}
}
