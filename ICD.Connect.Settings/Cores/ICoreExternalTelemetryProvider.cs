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

		[StaticPropertyTelemetry("ThemeName")]
		string ThemeName { get; }

		[StaticPropertyTelemetry("ThemeVersion")]
		string ThemeVersion { get; }

		[StaticPropertyTelemetry("ThemeInformationalVersion")]
		string ThemeInformationalVersion { get; }
	}
}