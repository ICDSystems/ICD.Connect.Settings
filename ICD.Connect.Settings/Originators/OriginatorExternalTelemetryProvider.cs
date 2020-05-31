using ICD.Common.Utils.Extensions;
using ICD.Connect.Telemetry.Providers.External;

namespace ICD.Connect.Settings.Originators
{
	public sealed class OriginatorExternalTelemetryProvider : AbstractExternalTelemetryProvider<IOriginator>, IOriginatorExternalTelemetryProvider
	{
		public string OriginatorType { get { return Parent.GetType().GetMinimalName(); } }
	}
}