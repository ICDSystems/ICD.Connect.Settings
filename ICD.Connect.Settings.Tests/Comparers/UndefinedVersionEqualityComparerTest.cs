using System;
using ICD.Connect.Settings.Comparers;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Comparers
{
	[TestFixture]
	public sealed class UndefinedVersionEqualityComparerTest
	{
		[Test]
		public void Equals()
		{
			Assert.IsTrue(UndefinedVersionEqualityComparer.Instance.Equals(new Version(0, 0), new Version(0, 0, 0, 0)));
			Assert.IsFalse(UndefinedVersionEqualityComparer.Instance.Equals(new Version(0, 0), new Version(1, 0, 0, 0)));
		}
	}
}
