namespace ICD.Connect.Settings.Core
{
	public abstract class AbstractCoreSettings : AbstractSettings, ICoreSettings
	{
		public abstract SettingsCollection OriginatorSettings { get; }
	}
}
