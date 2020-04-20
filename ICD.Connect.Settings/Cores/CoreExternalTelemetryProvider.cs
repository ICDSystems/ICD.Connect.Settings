using ICD.Common.Utils.Extensions;
using ICD.Connect.Telemetry;

namespace ICD.Connect.Settings.Cores
{
	public sealed class CoreExternalTelemetryProvider : AbstractExternalTelemetryProvider<ICore>,
	                                                    ICoreExternalTelemetryProvider
	{
		public string SoftwareVersion { get { return Parent.GetType().GetAssembly().GetName().Version.ToString(); } }

		public string SoftwareInformationalVersion
		{
			get
			{
				string version;
				return Parent.GetType().GetAssembly().TryGetInformationalVersion(out version) ? version : null;
			}
		}
	}
}
