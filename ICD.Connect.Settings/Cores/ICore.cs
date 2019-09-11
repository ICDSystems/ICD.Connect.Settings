using ICD.Connect.API.Attributes;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Cores
{
	[ApiClass("Core", "Contains the devices, panels, rooms, etc for a control system.")]
	public interface ICore : IOriginator
	{
		#region Properties

		/// <summary>
		/// Gets the stored originators.
		/// </summary>
		IOriginatorCollection<IOriginator> Originators { get; }

		/// <summary>
		/// Gets the configured localization.
		/// </summary>
		Localization.Localization Localization { get; }

		#endregion

		#region Settings

		/// <summary>
		/// Applies the settings to the Core instance.
		/// </summary>
		/// <param name="settings"></param>
		void ApplySettings(ICoreSettings settings);

		/// <summary>
		/// Copies the current state of the Core instance.
		/// </summary>
		/// <returns></returns>
		new ICoreSettings CopySettings();

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		void CopySettings(ICoreSettings settings);

		/// <summary>
		/// Loads settings from disk and updates the Settings property.
		/// </summary>
		void LoadSettings();

		#endregion
	}
}
