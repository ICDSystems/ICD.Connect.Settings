using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Logging.Activities;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.Utils;

namespace ICD.Connect.Settings.Originators
{
	public abstract class AbstractOriginator<T> : IOriginator<T>, IStateDisposable
		where T : ISettings, new()
	{
		/// <summary>
		/// Raised when the lifecycle state of the originator changes
		/// </summary>
		public event EventHandler<LifecycleStateEventArgs> OnLifecycleStateChanged;

		/// <summary>
		/// Raised when the name changes.
		/// </summary>
		public event EventHandler OnNameChanged;

		/// <summary>
		/// Raised when the disable state changes.
		/// </summary>
		public event EventHandler<BoolEventArgs> OnDisableStateChanged;

		private readonly List<Permission> m_Permissions;
		private readonly ILoggingContext m_Logger;
		private readonly IActivityContext m_Activities;

		private PermissionsManager m_CachedPermissionsManager;
		private ICore m_CachedCore;
		private string m_Name;
		private bool m_Disable;
		private eLifecycleState m_LifecycleState;

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
		/// The lifecycle state of the originator
		/// Describes the state in the instantiate/load/start/clear/dispose lifecycle
		/// </summary>
		public eLifecycleState LifecycleState
		{
			get { return m_LifecycleState; }
			private set
			{
				if (m_LifecycleState == value)
					return;

				m_LifecycleState = value;

				OnLifecycleStateChanged.Raise(this, new LifecycleStateEventArgs(m_LifecycleState));
			}
		}

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
				if (m_Name == value)
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
		public ILoggingContext Logger { get { return m_Logger; } }

		/// <summary>
		/// Activities for the originator.
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

		protected PermissionsManager PermissionsManager
		{
			get
			{
				return m_CachedPermissionsManager =
					       m_CachedPermissionsManager ?? ServiceProvider.TryGetService<PermissionsManager>();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractOriginator()
		{
			m_Permissions = new List<Permission>();
			m_Logger = new ServiceLoggingContext(this);
			m_Activities = new ActivityContext();

			Name = GetType().Name;
		}

		/// <summary>
		/// Deconstructor.
		/// </summary>
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
			{
				BuildStringRepresentationProperties((n, v) => builder.AppendProperty(n, v));
			}
			return builder.ToString();
		}

		/// <summary>
		/// Initializes the current telemetry state.
		/// </summary>
		public void InitializeTelemetry()
		{
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
			{
				addPropertyAndValue("Name", Name);

				if (!string.IsNullOrEmpty(CombineName) && CombineName != Name)
					addPropertyAndValue("CombineName", CombineName);
			}
		}

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		public IEnumerable<Permission> GetPermissions()
		{
			return m_Permissions.ToList();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases resources but also allows for finalizing without touching managed resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			LifecycleState = eLifecycleState.Disposed;
			OnLifecycleStateChanged = null;
			OnNameChanged = null;
			OnDisableStateChanged = null;

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
			settings.Uuid = Uuid;
			settings.Name = Name;
			settings.CombineName = CombineName;
			settings.Description = Description;
			settings.Hide = Hide;
			settings.Permissions = (GetPermissions() ?? Enumerable.Empty<Permission>()).ToList();
			settings.Order = Order;
			settings.Disable = Disable;

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

			LifecycleState = eLifecycleState.Loading;

			Id = settings.Id;
			Uuid = settings.Uuid == default(Guid) ? OriginatorUtils.GenerateUuid(Core, Id) : settings.Uuid;
			Name = string.IsNullOrEmpty(settings.Name) ? GetType().Name : settings.Name;
			CombineName = string.IsNullOrEmpty(settings.CombineName) ? Name : settings.CombineName;
			Description = settings.Description;
			Hide = settings.Hide;
			Order = settings.Order;
			Disable = settings.Disable;

			SetPermissions(settings.Permissions);

			ApplySettingsFinal(settings, factory);

			LifecycleState = eLifecycleState.Loaded;
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
			LifecycleState = eLifecycleState.Clearing;

			ClearSettingsFinal();

			// Don't clear ID - Causes lookup problems
			//Id = 0;
			//NewId = default(Guid);

			Name = GetType().Name;
			CombineName = GetType().Name;
			Description = null;
			Hide = false;
			Order = 0;
			Disable = false;

			SetPermissions(Enumerable.Empty<Permission>());

			LifecycleState = eLifecycleState.Cleared;
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

		/// <summary>
		/// Start settings for the origintor
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		public void StartSettings()
		{
			//Only Start Once
			if (LifecycleState == eLifecycleState.Started)
				return;

			LifecycleState = eLifecycleState.Starting;

			StartSettingsFinal();
			
			LifecycleState = eLifecycleState.Started;
		}

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected virtual void StartSettingsFinal()
		{
			
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (var node in OriginatorConsole.GetConsoleNodes(this))
				yield return node;
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
