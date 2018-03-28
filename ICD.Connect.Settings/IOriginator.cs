﻿using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
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

		/// <summary>
		/// The name that is used for the originator while in a combine space.
		/// </summary>
		[PublicAPI]
		string CombineName { get; set; }

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		[PublicAPI]
		IEnumerable<Permission> GetPermissions();

		/// <summary>
		/// When true this instance is serialized to the system config.
		/// </summary>
		bool Serialize { get; set; }

		/// <summary>
		/// Logger for the originator.
		/// </summary>
		ILoggerService Logger { get; }

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

	public interface IOriginator<TSettings> : IOriginator
		where TSettings : ISettings
	{
		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		new TSettings CopySettings();

		/// <summary>
		/// Copies the current instance properties to the settings instance. 
		/// </summary>
		/// <param name="settings"></param>
		void CopySettings(TSettings settings);

		/// <summary>
		/// Applies the settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		void ApplySettings(TSettings settings, IDeviceFactory factory);
	}

	public static class OriginatorExtensions
	{
		/// <summary>
		/// Gets the name for the originator based on the current room combine state.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="combine"></param>
		/// <returns></returns>
		public static string GetName(this IOriginator extends, bool combine)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (combine && !string.IsNullOrEmpty(extends.CombineName))
				return extends.CombineName;

			return extends.Name;
		}
	}
}
