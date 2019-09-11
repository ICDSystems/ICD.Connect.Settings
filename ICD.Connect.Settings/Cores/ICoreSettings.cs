using ICD.Connect.Settings.Header;

namespace ICD.Connect.Settings.Cores
{
	public interface ICoreSettings : ISettings
	{
		/// <summary>
		/// Gets the child originator settings collection.
		/// </summary>
		SettingsCollection OriginatorSettings { get; }

		/// <summary>
		/// Parses and returns only the header portion from the full config.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		ConfigurationHeader GetHeader(string config);
	}
}
