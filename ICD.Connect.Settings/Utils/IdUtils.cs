using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.Utils
{
	public enum eSubsystem
	{
		Ports = 1,
		Devices = 2,
		Connections = 3,
		StaticRoutes = 4,
		Sources = 5,
		Destinations = 6,
		Partitions = 7,
		Rooms = 8,
		VolumePoints = 9,
		ConferencePoints = 10,
		Cells = 11
	}

	public static class IdUtils
	{
		public const int ID_CORE = 1;

		public const int ID_THEME = 100;
		public const int ID_ROUTING_GRAPH = 200;
		public const int ID_PARTITION_MANAGER = 300;
		public const int ID_TELEMETRY = 400;

		private const int MULTIPLIER_SUBSYSTEM = 10 * 1000 * 1000;

		/// <summary>
		/// Gets the subsystem id start from the subsystem
		/// </summary>
		/// <param name="subsystem"></param>
		/// <returns></returns>
		public static int GetSubsystemId(eSubsystem subsystem)
		{
			return (int)subsystem * MULTIPLIER_SUBSYSTEM;
		}

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

			IcdHashSet<int> existing = existingIds.Where(e => e >= start).ToIcdHashSet();
			return Enumerable.Range(start, int.MaxValue - start)
			                 .First(i => !existing.Contains(i));
		}

		/// <summary>
		/// Gets a new, unique id given a sequence of existing ids and a subsystem
		/// </summary>
		/// <param name="existingIds"></param>
		/// <param name="subsystem"></param>
		/// <returns></returns>
		public static int GetNewId(IEnumerable<int> existingIds, eSubsystem subsystem)
		{
			if (existingIds == null)
				throw new ArgumentNullException("existingIds");

			return GetNewId(existingIds, GetSubsystemId(subsystem));
		}

		public static int GetNewRoomId(IEnumerable<int> existingRoomIds)
		{
			if (existingRoomIds == null)
				throw new ArgumentNullException("existingRoomIds");

			return GetNewId(existingRoomIds, eSubsystem.Rooms);
		}
	}
}
