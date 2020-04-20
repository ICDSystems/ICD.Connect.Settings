using System;
using System.Collections.Generic;
using ICD.Connect.Telemetry;
using ICD.Connect.Telemetry.Attributes;

namespace ICD.Connect.Settings.Cores
{
	public interface ICoreExternalTelemetryProvider : IExternalTelemetryProvider
	{
		[EventTelemetry("OnOriginatorIdsChanged")]
		event EventHandler OnOriginatorIdsChanged;

		[StaticPropertyTelemetry("SoftwareVersion")]
		string SoftwareVersion { get; }

		[StaticPropertyTelemetry("SoftwareInformationalVersion")]
		string SoftwareInformationalVersion { get; }

		[DynamicPropertyTelemetry("OriginatorIds", null, "OnOriginatorIdsChanged")]
		IEnumerable<Guid> OriginatorIds { get; }
	}
}
