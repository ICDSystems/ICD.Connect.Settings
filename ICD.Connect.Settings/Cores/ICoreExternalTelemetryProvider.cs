using System;
using System.Collections.Generic;
using ICD.Connect.Telemetry;
using ICD.Connect.Telemetry.Attributes;
using ICD.Connect.Telemetry.Nodes.External;

namespace ICD.Connect.Settings.Cores
{
	public interface ICoreExternalTelemetryProvider : IExternalTelemetryProvider
	{
		[EventTelemetry("OnOriginatorIdsChanged")]
		event EventHandler OnOriginatorIdsChanged;

		[PropertyTelemetry("SoftwareVersion", null, null)]
		string SoftwareVersion { get; }

		[PropertyTelemetry("SoftwareInformationalVersion", null, null)]
		string SoftwareInformationalVersion { get; }

		[PropertyTelemetry("OriginatorIds", null, "OnOriginatorIdsChanged")]
		IEnumerable<Guid> OriginatorIds { get; }
	}
}
