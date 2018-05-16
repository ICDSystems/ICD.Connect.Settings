namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCoreSettings : AbstractSettings, ICoreSettings
	{
		public abstract SettingsCollection OriginatorSettings { get; }
	}
}
