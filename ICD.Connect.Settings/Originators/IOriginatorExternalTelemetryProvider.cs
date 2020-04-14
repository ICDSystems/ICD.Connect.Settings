using ICD.Connect.Telemetry;
using ICD.Connect.Telemetry.Attributes;

namespace ICD.Connect.Settings.Originators
{
	public interface IOriginatorExternalTelemetryProvider : IExternalTelemetryProvider
	{
		[StaticPropertyTelemetry("OriginatorType")]
		string OriginatorType { get; }
	}
}