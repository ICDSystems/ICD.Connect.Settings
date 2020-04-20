using ICD.Connect.Telemetry;
using ICD.Connect.Telemetry.Attributes;

namespace ICD.Connect.Settings.Cores
{
	public interface ICoreExternalTelemetryProvider : IExternalTelemetryProvider
	{
		[StaticPropertyTelemetry("SoftwareVersion")]
		string SoftwareVersion { get; }

		[StaticPropertyTelemetry("SoftwareInformationalVersion")]
		string SoftwareInformationalVersion { get; }
	}
}
