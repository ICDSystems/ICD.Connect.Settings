using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings
{
	public static class IdUtils
	{
		public const int ID_CORE = 1;

		public const int ID_THEME = 100;
		public const int ID_ROUTING_GRAPH = 200;
		public const int ID_PARTITION_MANAGER = 300;

		public const int SUBSYSTEM_PORTS = 1;
		public const int SUBSYSTEM_DEVICES = 2;
		public const int SUBSYSTEM_PANELS = 3;
		public const int SUBSYSTEM_CONNECTIONS = 4;
		public const int SUBSYSTEM_STATIC_ROUTES = 5;
		public const int SUBSYSTEM_SOURCES = 6;
		public const int SUBSYSTEM_DESTINATIONS = 7;
		public const int SUBSYSTEM_PARTITIONS = 8;

		private const int MULTIPLIER_ROOM = 1000;
		private const int MULTIPLIER_SUBSYSTEM = 100 * 1000;

		/// <summary>
		/// Gets a new, unique id given a sequence of existing ids.
		/// </summary>
		/// <param name="existingIds"></param>
		/// <returns></returns>
		public static int GetNewId(IEnumerable<int> existingIds)
		{
			if (existingIds == null)
				throw new ArgumentNullException("existingIds");

			return GetNewId(existingIds, 1);
		}

		/// <summary>
		/// Gets a new, unique id given a sequence of existing ids and a start value.
		/// </summary>
		/// <param name="existingIds"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		public static int GetNewId(IEnumerable<int> existingIds, int start)
		{
			if (existingIds == null)
				throw new ArgumentNullException("existingIds");

			start = MathUtils.Clamp(start, 1, int.MaxValue);

			IcdHashSet<int> existing = existingIds.Where(e => e >= start).ToHashSet();
			return Enumerable.Range(start, int.MaxValue - start)
			                 .First(i => !existing.Contains(i));
		}

		/// <summary>
		/// Gets a new, unique id given a sequence of existing ids, a subsystem id and a room id.
		/// </summary>
		/// <param name="existingIds"></param>
		/// <param name="subsystemId"></param>
		/// <param name="roomId"></param>
		/// <returns></returns>
		public static int GetNewId(IEnumerable<int> existingIds, int subsystemId, int roomId)
		{
			if (existingIds == null)
				throw new ArgumentNullException("existingIds");

			int start = subsystemId + roomId;
			return GetNewId(existingIds, start);
		}

		public static int GetSubsystemId(int subsystemNumber)
		{
			return subsystemNumber * MULTIPLIER_SUBSYSTEM;
		}

		public static int GetRoomId(int roomNumber)
		{
			return roomNumber * MULTIPLIER_ROOM;
		}

		public static int GetNewRoomId(IEnumerable<int> existingRoomIds)
		{
			if (existingRoomIds == null)
				throw new ArgumentNullException("existingRoomIds");

			IcdHashSet<int> existing = existingRoomIds.ToHashSet();

			return Enumerable.Range(1, int.MaxValue)
			                 .Select(i => GetRoomId(i))
			                 .First(i => !existing.Contains(i));
		}
	}
}
