namespace ICD.Connect.Settings.Core
{
	public interface ICoreSettings : ISettings
	{
		/// <summary>
		/// Gets the child originator settings collection.
		/// </summary>
		SettingsCollection OriginatorSettings { get; }
	}
}
