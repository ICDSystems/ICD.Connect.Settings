using ICD.Connect.Settings.Utils;
using NUnit.Framework;

namespace ICD.Connect.Settings.Tests.Utils
{
	[TestFixture]
    public sealed class IdUtilsTest
    {
		[TestCase(4, 1, 2, 3)]
		public void GetNewIdTest(int expected, params int[] existing)
		{
			Assert.AreEqual(expected, IdUtils.GetNewId(existing));
		}

		[TestCase(10, 10, 2, 3, 11)]
		public void GetNewIdStartTest(int expected, int start, params int[] existing)
		{
			Assert.AreEqual(expected, IdUtils.GetNewId(existing, start));
		}

		[TestCase(20000002, eSubsystem.Devices, 1000, 20000000, 20000001, 20000003)]
		public void GetNewIdSubsystemRoomTest(int expected, eSubsystem subsystem, params int[] existing)
		{
			Assert.AreEqual(expected, IdUtils.GetNewId(existing, subsystem));
		}

		[TestCase(eSubsystem.Ports, 10000000)]
		[TestCase(eSubsystem.Devices, 20000000)]
		[TestCase(eSubsystem.Cells, 110000000)]
		public void GetSubsystemIdTest(eSubsystem subsystem, int expected)
		{
			Assert.AreEqual(expected, IdUtils.GetSubsystemId(subsystem));
		}

		[TestCase(80000000)]
		[TestCase(80000004, 80000000, 80000001, 80000002, 80000003, 80000005)]
		public void GetNewRoomIdTest(int expected, params int[] existing)
		{
			Assert.AreEqual(expected, IdUtils.GetNewRoomId(existing));
		}
	}
}
