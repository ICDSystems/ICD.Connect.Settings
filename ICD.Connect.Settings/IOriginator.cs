using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// IOriginator represents an object that has settings.
	/// </summary>
	public interface IOriginator
	{
		/// <summary>
		/// Called when the settings start clearing.
		/// </summary>
		[PublicAPI]
		event EventHandler OnSettingsClearing;

		/// <summary>
		/// Called when the settings have been cleared.
		/// </summary>
		[PublicAPI]
		event EventHandler OnSettingsCleared;

		/// <summary>
		/// Called when settings have been applied to the originator.
		/// </summary>
		[PublicAPI]
		event EventHandler OnSettingsApplied;

		#region Properties

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		[PublicAPI]
		int Id { get; set; }

		/// <summary>
		/// The name of the originator.
		/// </summary>
		[PublicAPI]
		string Name { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		ISettings CopySettings();

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		[PublicAPI]
		void CopySettings(ISettings settings);

		/// <summary>
		/// Applies the settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		[PublicAPI]
		void ApplySettings(ISettings settings, IDeviceFactory factory);

		/// <summary>
		/// Resets the originator to a default state.
		/// </summary>
		[PublicAPI]
		void ClearSettings();

		#endregion
	}
}
