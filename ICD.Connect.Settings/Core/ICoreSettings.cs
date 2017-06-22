namespace ICD.Connect.Settings.Core
{
	public interface ICoreSettings : ISettings
	{
		SettingsCollection OriginatorSettings { get; }

		void ParseXml(string xml);
	}
}
