﻿using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Info;
using ICD.Connect.API.Nodes;
using ICD.Connect.API.Proxies;
using ICD.Connect.Settings.Originators;
using ICD.Connect.Telemetry;

namespace ICD.Connect.Settings.Proxies
{
	public abstract class AbstractProxyOriginator<TSettings> : AbstractProxy, IProxyOriginator<TSettings>
		where TSettings : IProxySettings
	{
		/// <summary>
		/// Called when the settings start clearing.
		/// </summary>
		public event EventHandler OnSettingsClearing;

		/// <summary>
		/// Called when the settings have been cleared.
		/// </summary>
		public event EventHandler OnSettingsCleared;

		/// <summary>
		/// Called when settings have been applied to the originator.
		/// This means the originator has finished loading.
		/// </summary>
		public event EventHandler OnSettingsApplied;
		public event EventHandler OnNameChanged;
		public event EventHandler OnRequestTelemetryRebuild;

		private ILoggerService m_CachedLogger;

		#region Properties

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The name of the originator.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The name that is used for the originator while in a combine space.
		/// </summary>
		public string CombineName { get; set; }

		/// <summary>
		/// Human readable text describing the originator.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Controls the visibility of the originator to the end user.
		/// Useful for hiding logical switchers, duplicate sources, etc.
		/// </summary>
		public bool Hide { get; set; }

		/// <summary>
		/// When true this instance is serialized to the system config.
		/// </summary>
		public bool Serialize { get; set; }

		/// <summary>
		/// Logger for the originator.
		/// </summary>
		[Obsolete]
		public ILoggerService Logger
		{
			get { return m_CachedLogger = m_CachedLogger ?? ServiceProvider.TryGetService<ILoggerService>(); }
		}

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return string.IsNullOrEmpty(Name) ? GetType().GetNameWithoutGenericArity() : Name; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return string.Empty; } }

		
		public ITelemetryCollection Telemetry { get; [UsedImplicitly] set; }

		#endregion

		#region Methods

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		public IEnumerable<Permission> GetPermissions()
		{
			yield break;
		}

		#endregion

		#region Settings

		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		ISettings IOriginator.CopySettings()
		{
			return CopySettings();
		}

		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		public TSettings CopySettings()
		{
			TSettings output = ReflectionUtils.CreateInstance<TSettings>();
			CopySettings(output);
			return output;
		}

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		void IOriginator.CopySettings(ISettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			if (!(settings is TSettings))
			{
				string message = string.Format("Can not use {0} with {1}", settings.GetType().Name, GetType().Name);
				throw new ArgumentException(message);
			}

			CopySettings((TSettings)settings);
		}

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		public void CopySettings(TSettings settings)
		{
			settings.Id = Id;
			settings.Name = Name;
			settings.CombineName = CombineName;
			settings.Description = Description;
			settings.Hide = Hide;

			CopySettingsFinal(settings);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void CopySettingsFinal(TSettings settings)
		{
		}

		/// <summary>
		/// Applies the settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		void IOriginator.ApplySettings(ISettings settings, IDeviceFactory factory)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			if (!(settings is TSettings))
			{
				string message = string.Format("Can not use {0} with {1}", settings.GetType().Name, GetType().Name);
				throw new ArgumentException(message);
			}

			ApplySettings((TSettings)settings, factory);
		}

		/// <summary>
		/// Applies the settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		public void ApplySettings(TSettings settings, IDeviceFactory factory)
		{
			ClearSettings();

			Id = settings.Id;
			Name = settings.Name;
			CombineName = settings.CombineName;
			Description = settings.Description;
			Hide = settings.Hide;

			ApplySettingsFinal(settings, factory);

			OnSettingsApplied.Raise(this);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected virtual void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
		}

		/// <summary>
		/// Resets the originator to a default state.
		/// </summary>
		public void ClearSettings()
		{
			OnSettingsClearing.Raise(this);

			ClearSettingsFinal();

			Id = 0;
			Name = null;
			CombineName = null;
			Description = null;
			Hide = false;

			OnSettingsCleared.Raise(this);
		}


		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected virtual void ClearSettingsFinal()
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnSettingsApplied = null;
			OnSettingsCleared = null;
			OnSettingsClearing = null;

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Override to build initialization commands on top of the current class info.
		/// </summary>
		/// <param name="command"></param>
		protected override void Initialize(ApiClassInfo command)
		{
			base.Initialize(command);

			ApiCommandBuilder.UpdateCommand(command)
			                 .GetProperty(OriginatorApi.PROPERTY_NAME)
			                 .GetProperty(OriginatorApi.PROPERTY_COMBINE_NAME)
							 .GetProperty(OriginatorApi.PROPERTY_DESCRIPTION)
							 .GetProperty(OriginatorApi.PROPERTY_HIDE)
			                 .Complete();
		}

		/// <summary>
		/// Updates the proxy with a property result.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="result"></param>
		protected override void ParseProperty(string name, ApiResult result)
		{
			base.ParseProperty(name, result);

			switch (name)
			{
				case OriginatorApi.PROPERTY_NAME:
					Name = result.GetValue<string>();
					break;

				case OriginatorApi.PROPERTY_COMBINE_NAME:
					CombineName = result.GetValue<string>();
					break;

				case OriginatorApi.PROPERTY_DESCRIPTION:
					Description = result.GetValue<string>();
					break;

				case OriginatorApi.PROPERTY_HIDE:
					Hide = result.GetValue<bool>();
					break;
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			return OriginatorConsole.GetConsoleNodes(this);
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public virtual void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			OriginatorConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			return OriginatorConsole.GetConsoleCommands(this);
		}

		#endregion
	}
}
