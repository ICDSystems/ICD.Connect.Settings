using System;
using ICD.Common.Utils;
using ICD.Connect.Settings.Cores;

namespace ICD.Connect.Settings.Utils
{
	public static class OriginatorUtils
	{
		/// <summary>
		/// Generates a UUID based on the core UUID and the originator ID.
		/// </summary>
		/// <returns></returns>
		public static Guid GenerateUuid(ICore core, int id)
		{
			Guid idGuid = GuidUtils.GenerateSeeded(id);
			return GuidUtils.Combine(core.Uuid, idGuid);
		}
	}
}
