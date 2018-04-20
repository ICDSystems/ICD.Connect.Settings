using ICD.Connect.API.Attributes;

namespace ICD.Connect.Settings.Core
{
	[ApiClass("Core", "Contains the devices, panels, rooms etc for a control system.")]
	public interface ICore : IOriginator
	{
		#region Properties

		IOriginatorCollection<IOriginator> Originators { get; }

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
		/// Loads settings from disk and updates the Settings property.
		/// </summary>
		void LoadSettings();

		#endregion

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		void CopySettings(ICoreSettings settings);
	}
}
