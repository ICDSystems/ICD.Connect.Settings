using System;
using ICD.Connect.Telemetry.Attributes;
using ICD.Connect.Telemetry.Providers;

namespace ICD.Connect.Settings.Organizations
{
	public sealed class Organization : ITelemetryProvider
	{
		#region Properties

		/// <summary>
		/// Gets/sets the ID of the organization.
		/// </summary>
		[PropertyTelemetry("Id", null, null)]
		public int Id { get; set; }

		/// <summary>
		/// Gets/sets the name of the organization.
		/// </summary>
		[PropertyTelemetry("Name", null, null)]
		public string Name { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the current telemetry state.
		/// </summary>
		public void InitializeTelemetry()
		{
		}

		#endregion

		#region Settings

		/// <summary>
		/// Reverts to defaults.
		/// </summary>
		public void ClearSettings()
		{
			Id = 0;
			Name = null;
		}

		/// <summary>
		/// Copies the current state onto the given settings instance.
		/// </summary>
		/// <param name="settings"></param>
		public void CopySettings(OrganizationSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			settings.Id = Id;
			settings.Name = Name;
		}

		/// <summary>
		/// Applies the given settings to the current state.
		/// </summary>
		/// <param name="settings"></param>
		public void ApplySettings(OrganizationSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			ClearSettings();

			Id = settings.Id;
			Name = settings.Name;
		}

		#endregion
	}
}