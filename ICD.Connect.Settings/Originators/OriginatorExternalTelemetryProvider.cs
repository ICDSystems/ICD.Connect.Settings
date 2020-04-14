using ICD.Common.Utils.Extensions;
using ICD.Connect.Telemetry;

namespace ICD.Connect.Settings.Originators
{
	public sealed class OriginatorExternalTelemetryProvider : IOriginatorExternalTelemetryProvider
	{
        private IOriginator m_Parent;

		/// <summary>
		/// Sets the parent telemetry provider that this instance extends.
		/// </summary>
		/// <param name="provider"></param>
		public void SetParent(ITelemetryProvider provider)
		{
			m_Parent = provider as IOriginator;
		}

		public string OriginatorType { get { return m_Parent.GetType().GetMinimalName(); } }
	}
}