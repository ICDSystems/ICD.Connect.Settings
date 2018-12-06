using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Core;
using ICD.Connect.Telemetry;
using ICD.Connect.Telemetry.Nodes;

namespace ICD.Connect.Settings
{
	public abstract class AbstractOriginator<T> : IOriginator<T>, IStateDisposable
		where T : ISettings, new()
	{
		/// <summary>
		/// Called when the settings start clearing.
		/// </summary>
		public event EventHandler OnSettingsClearing;

		/// <summary>
		/// Raised when settings have been cleared.
		/// </summary>
		public event EventHandler OnSettingsCleared;

		/// <summary>
		/// Raised when settings have been applied to the originator.
		/// </summary>
		public event EventHandler OnSettingsApplied;

		public event EventHandler OnRequestTelemetryRebuild;

		private readonly List<Permission> m_Permissions;

		private ILoggerService m_CachedLogger;
		private PermissionsManager m_CachedPermissionsManager;

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
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

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

		protected PermissionsManager PermissionsManager
		{
			get
			{
				return m_CachedPermissionsManager =
					       m_CachedPermissionsManager ?? ServiceProvider.TryGetService<PermissionsManager>();
			}
		}

		public ITelemetryCollection Telemetry { get; private set; }

		#endregion

		protected AbstractOriginator()
		{
			m_Permissions = new List<Permission>();
		}

		~AbstractOriginator()
		{
			Dispose(false);
		}

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Gets the string representation for this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);
			BuildStringRepresentationProperties(builder.AppendProperty);
			return builder.ToString();
		}

		/// <summary>
		/// Override to add additional properties to the ToString representation.
		/// </summary>
		/// <param name="addPropertyAndValue"></param>
		protected virtual void BuildStringRepresentationProperties(Action<string, object> addPropertyAndValue)
		{
			if (Id != 0)
				addPropertyAndValue("Id", Id);

			if (!string.IsNullOrEmpty(Name) && Name != GetType().Name)
				addPropertyAndValue("Name", Name);
		}

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		public IEnumerable<Permission> GetPermissions()
		{
			return m_Permissions.ToList();
		}

		public void Log(eSeverity severity, string message)
		{
			Logger.AddEntry(severity, "{0} - {1}", this, message);
		}

		public void Log(eSeverity severity, string message, params object[] args)
		{
			message = string.Format(message, args);
			Log(severity, message);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases resources but also allows for finalizing without touching managed resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			OnSettingsClearing = null;
			OnSettingsCleared = null;
			OnSettingsApplied = null;

			if (!IsDisposed)
				DisposeFinal(disposing);
			IsDisposed = IsDisposed || disposing;
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void DisposeFinal(bool disposing)
		{
			ClearSettings();
		}

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		private void SetPermissions(IEnumerable<Permission> permissions)
		{
			if (permissions == null)
				throw new ArgumentNullException("permissions");

			m_Permissions.Clear();
			m_Permissions.AddRange(permissions);

		    if (PermissionsManager != null)
		        PermissionsManager.SetObjectPermissions(this, m_Permissions);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		public T CopySettings()
		{
			T output = ReflectionUtils.CreateInstance<T>();
			CopySettings(output);
			return output;
		}

		/// <summary>
		/// Copies the current instance properties to the settings instance. 
		/// </summary>
		/// <param name="settings"></param>
		public void CopySettings(T settings)
		{
			settings.Id = Id;
			settings.Name = Name;
			settings.CombineName = CombineName;
			settings.Description = Description;
			settings.Hide = Hide;
			settings.Permissions = (GetPermissions() ?? Enumerable.Empty<Permission>()).ToList();

			CopySettingsFinal(settings);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected virtual void CopySettingsFinal(T settings)
		{
		}

		/// <summary>
		/// Applies the settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		public void ApplySettings(T settings, IDeviceFactory factory)
		{
			ClearSettings();

			Id = settings.Id;
			Name = settings.Name;
			CombineName = settings.CombineName;
			Description = settings.Description;
			Hide = settings.Hide;

			SetPermissions(settings.Permissions ?? Enumerable.Empty<Permission>());

			ApplySettingsFinal(settings, factory);

			Telemetry = new StaticTelemetryNodeItem<object>(Name, null);
			TelemetryUtils.InstantiateTelemetry(this);

			OnSettingsApplied.Raise(this);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected virtual void ApplySettingsFinal(T settings, IDeviceFactory factory)
		{
		}

		/// <summary>
		/// Resets the originator to a default state.
		/// </summary>
		public void ClearSettings()
		{
			OnSettingsClearing.Raise(this);

			ClearSettingsFinal();

			// Don't clear ID - Causes lookup problems
			//Id = 0;

			Name = null;
			CombineName = null;
			Description = null;
			Hide = false;

			SetPermissions(Enumerable.Empty<Permission>());

			OnSettingsCleared.Raise(this);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected virtual void ClearSettingsFinal()
		{
		}

		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		ISettings IOriginator.CopySettings()
		{
			return CopySettings();
		}

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		void IOriginator.CopySettings(ISettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			if (!(settings is T))
			{
				string message = string.Format("Can not use {0} with {1}", settings.GetType().Name, GetType().Name);
				throw new ArgumentException(message);
			}

			CopySettings((T)settings);
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

			if (!(settings is T))
			{
				string message = string.Format("Can not use {0} with {1}", settings.GetType().Name, GetType().Name);
				throw new ArgumentException(message);
			}

			ApplySettings((T)settings, factory);
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

			var tableBuilder = new TableBuilder("Name", "Value");

			foreach (ITelemetryItem node in Telemetry.GetChildren())
			{
				IFeedbackTelemetryItem feedbackItem = node as IFeedbackTelemetryItem;
				if(feedbackItem != null)
					tableBuilder.AddRow(feedbackItem.Name, feedbackItem.Value);

				IManagementTelemetryItem managementItem = node as IManagementTelemetryItem;
				if(managementItem != null)
					tableBuilder.AddRow(managementItem.Name, "Method Telemetry");
			}

			IcdConsole.PrintLine(eConsoleColor.Magenta, "TELEMETRY for {0}", this);
			IcdConsole.PrintLine(eConsoleColor.Magenta, tableBuilder.ToString());
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
