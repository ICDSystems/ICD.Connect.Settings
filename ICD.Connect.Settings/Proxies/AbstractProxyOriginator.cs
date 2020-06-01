using System;
using System.Collections.Generic;
using ICD.Common.Logging.Activities;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Connect.API;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Info;
using ICD.Connect.API.Nodes;
using ICD.Connect.API.Proxies;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.Originators;

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

		/// <summary>
		/// Called when this originator changes names.
		/// </summary>
		public event EventHandler OnNameChanged;

		/// <summary>
		/// Raised when the disable state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnDisableStateChanged;

		private readonly ILoggingContext m_Logger;
		private readonly IActivityContext m_Activities;

		private string m_Name;
		private bool m_Disable;
		private ICore m_CachedCore;

		#region Properties

		/// <summary>
		/// Gets the parent core instance.
		/// </summary>
		public ICore Core { get { return m_CachedCore = m_CachedCore ?? ServiceProvider.GetService<ICore>(); } }

		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public abstract string Category { get; }

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		public Guid Uuid { get; set; }

		/// <summary>
		/// The name of the originator.
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set
			{
				if (value == m_Name)
					return;

				m_Name = value;

				OnNameChanged.Raise(this);
			}
		}

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
		/// Shorthand for disabling an instance in the system.
		/// </summary>
		public bool Disable
		{
			get { return m_Disable; }
			set
			{
				if (value == m_Disable)
					return;

				m_Disable = value;

				OnDisableStateChanged.Raise(this, new BoolEventArgs(m_Disable));
			}
		}

		/// <summary>
		/// Specifies custom ordering of the instance to the end user.
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// When true this instance is serialized to the system config.
		/// </summary>
		public bool Serialize { get; set; }

		/// <summary>
		/// Logger for the originator.
		/// </summary>
		public ILoggingContext Logger { get { return m_Logger; } }

		/// <summary>
		/// Gets the activities for this instance.
		/// </summary>
		public IActivityContext Activities { get { return m_Activities; } }

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return string.IsNullOrEmpty(Name) ? GetType().GetNameWithoutGenericArity() : Name; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return string.Empty; } }
		
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractProxyOriginator()
		{
			m_Logger = new ServiceLoggingContext(this);
			m_Activities = new ActivityContext();
		}

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
			settings.Uuid = Uuid;
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
			Uuid = settings.Uuid;
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
			Uuid = default(Guid);
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
			OnNameChanged = null;
			OnDisableStateChanged = null;

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
