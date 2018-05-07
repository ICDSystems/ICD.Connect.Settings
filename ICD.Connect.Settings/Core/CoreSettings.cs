using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Settings.Core
{
	[KrangSettings("Core", typeof(Core))]
	public sealed class CoreSettings : AbstractCoreSettings
	{
		private readonly SettingsCollection m_OriginatorSettings;

		public override SettingsCollection OriginatorSettings { get { return m_OriginatorSettings; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public CoreSettings()
		{
			m_OriginatorSettings = new SettingsCollection();
		}
	}
}
