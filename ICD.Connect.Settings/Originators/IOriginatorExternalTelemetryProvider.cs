using ICD.Connect.Telemetry;
using ICD.Connect.Telemetry.Attributes;
using ICD.Connect.Telemetry.Nodes.External;

namespace ICD.Connect.Settings.Originators
{
	public interface IOriginatorExternalTelemetryProvider : IExternalTelemetryProvider
	{
		[PropertyTelemetry("OriginatorType", null, null)]
		string OriginatorType { get; }
	}
}