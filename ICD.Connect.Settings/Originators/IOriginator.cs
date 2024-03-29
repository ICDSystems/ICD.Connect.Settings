﻿using System;
using System.Collections.Generic;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Permissions;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.API.Attributes;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Telemetry.Attributes;
using ICD.Connect.Telemetry.Providers;

namespace ICD.Connect.Settings.Originators
{
	/// <summary>
	/// IOriginator represents an object that has settings.
	/// </summary>
	public interface IOriginator : IConsoleNode, IActivityTelemetryProvider
	{
		/// <summary>
		/// Raised when the lifecycle state of the originator changes
		/// </summary>
		event EventHandler<LifecycleStateEventArgs> OnLifecycleStateChanged;


		/// <summary>
		/// Called when this originator changes names.
		/// </summary>
		[PublicAPI]
		[EventTelemetry(OriginatorTelemetryNames.NAME_CHANGED)]
		event EventHandler OnNameChanged;

		/// <summary>
		/// Raised when the disable state changes.
		/// </summary>
		event EventHandler<BoolEventArgs> OnDisableStateChanged;

		#region Properties

		/// <summary>
		/// Gets the parent core instance.
		/// </summary>
		ICore Core { get; }

		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		[PropertyTelemetry(OriginatorTelemetryNames.CATEGORY, null, null)]
		string Category { get; }

		/// <summary>
		/// The lifecycle state of the originator
		/// Describes the state in the instantiate/load/start/clear/dispose lifecycle
		/// </summary>
		eLifecycleState LifecycleState { get; }

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		[ApiProperty(OriginatorApi.PROPERTY_ID, OriginatorApi.HELP_PROPERTY_ID)]
		[PropertyTelemetry(OriginatorTelemetryNames.ID, null, null)]
		int Id { get; set; }

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		[ApiProperty(OriginatorApi.PROPERTY_UUID, OriginatorApi.HELP_PROPERTY_UUID)]
		[PropertyTelemetry(OriginatorTelemetryNames.UUID, null, null)]
		[TelemetryCollectionIdentity]
		Guid Uuid { get; set; }

		/// <summary>
		/// The name of the originator.
		/// </summary>
		[ApiProperty(OriginatorApi.PROPERTY_NAME, OriginatorApi.HELP_PROPERTY_NAME)]
		[PropertyTelemetry(OriginatorTelemetryNames.NAME, null, OriginatorTelemetryNames.NAME_CHANGED)]
		string Name { get; set; }

		/// <summary>
		/// The name that is used for the originator while in a combine space.
		/// </summary>
		[ApiProperty(OriginatorApi.PROPERTY_COMBINE_NAME, OriginatorApi.HELP_PROPERTY_COMBINE_NAME)]
		string CombineName { get; set; }

		/// <summary>
		/// Human readable text describing the originator.
		/// </summary>
		[ApiProperty(OriginatorApi.PROPERTY_DESCRIPTION, OriginatorApi.HELP_PROPERTY_DESCRIPTION)]
		string Description { get; set; }

		/// <summary>
		/// Controls the visibility of the originator to the end user.
		/// Useful for hiding logical switchers, duplicate sources, etc.
		/// </summary>
		[ApiProperty(OriginatorApi.PROPERTY_HIDE, OriginatorApi.HELP_PROPERTY_HIDE)]
		bool Hide { get; set; }

		/// <summary>
		/// Shorthand for disabling an instance in the system.
		/// </summary>
		bool Disable { get; set; }

		/// <summary>
		/// Specifies custom ordering of the instance to the end user.
		/// </summary>
		int Order { get; set; }

		/// <summary>
		/// When true this instance is serialized to the system config.
		/// </summary>
		bool Serialize { get; set; }

		/// <summary>
		/// Logger for the originator.
		/// </summary>
		ILoggingContext Logger { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		[PublicAPI]
		IEnumerable<Permission> GetPermissions();

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

		/// <summary>
		/// Start settings for the origintor
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		[PublicAPI]
		void StartSettings();

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

		/// <summary>
		/// Returns a string representation of the originator with ID and Name info.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string ToStringShorthand(this IOriginator extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			ReprBuilder builder = new ReprBuilder(extends);

			if (extends.Id != 0)
				builder.AppendProperty("Id", extends.Id);

			if (!string.IsNullOrEmpty(extends.Name) && extends.Name != extends.GetType().Name)
				builder.AppendProperty("Name", extends.Name);

			return builder.ToString();
		}
	}
}
