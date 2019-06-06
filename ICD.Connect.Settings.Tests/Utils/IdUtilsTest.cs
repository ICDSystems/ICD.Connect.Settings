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

		[TestCase(201002, 200000, 1000, 201000, 201001, 201003)]
		public void GetNewIdSubsystemRoomTest(int expected, int subsystemId, int roomId, params int[] existing)
		{
			Assert.AreEqual(expected, IdUtils.GetNewId(existing, subsystemId, roomId));
		}

		[TestCase(1, 100000)]
		[TestCase(2, 200000)]
		[TestCase(10, 1000000)]
		public void GetSubsystemIdTest(int subsystemNumber, int expected)
		{
			Assert.AreEqual(expected, IdUtils.GetSubsystemId(subsystemNumber));
		}

		[TestCase(1, 1000)]
		[TestCase(2, 2000)]
		[TestCase(10, 10000)]
		public void GetRoomIdTest(int roomNumber, int expected)
		{
			Assert.AreEqual(expected, IdUtils.GetRoomId(roomNumber));
		}

		[TestCase(1000)]
		[TestCase(3000, 1000, 2000, 4000)]
		public void GetNewRoomIdTest(int expected, params int[] existing)
		{
			Assert.AreEqual(expected, IdUtils.GetNewRoomId(existing));
		}
	}
}
