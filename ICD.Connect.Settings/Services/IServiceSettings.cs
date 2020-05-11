namespace ICD.Connect.Settings.Services
{
	public interface IServiceSettings : ISettings
	{
		/// <summary>
		/// Gets the provider settings for the service.
		/// </summary>
		SettingsCollection ProviderSettings { get; }
	}
}
