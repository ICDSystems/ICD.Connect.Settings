using System.Collections.Generic;
using ICD.Connect.Settings.Header;
using ICD.Connect.Settings.Validation;

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

		/// <summary>
		/// Validates the core settings as a whole.
		/// </summary>
		/// <returns></returns>
		IEnumerable<SettingsValidationResult> Validate();
	}
}
