using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Header;

namespace ICD.Connect.Settings.Core
{
	[KrangSettings("Core", typeof(Core))]
	public sealed class CoreSettings : AbstractCoreSettings
	{
		private readonly SettingsCollection m_OriginatorSettings;

		public override SettingsCollection OriginatorSettings { get { return m_OriginatorSettings; } }

		/// <summary>
		/// Parses and returns only the header portion from the full XML config.
		/// </summary>
		/// <param name="configXml"></param>
		/// <returns></returns>
		public override ConfigurationHeader GetHeader(string configXml)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public CoreSettings()
		{
			m_OriginatorSettings = new SettingsCollection();
		}
	}
}
