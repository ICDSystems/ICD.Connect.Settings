using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Permissions;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings
{
	public abstract class AbstractOriginator<T> : IOriginator, IStateDisposable
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
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		public IEnumerable<Permission> Permissions { get; private set; }

		/// <summary>
		/// When true this instance is serialized to the system config.
		/// </summary>
		public bool Serialize { get; set; }

		/// <summary>
		/// Logger for the originator.
		/// </summary>
		public ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		#endregion

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
		/// Clears the permissions stored in the PermissionsManager for this object
		/// and sets them to the current permissions of the object.
		/// </summary>
		public void ResetPermissions()
		{
			var permissionsManager = ServiceProvider.TryGetService<PermissionsManager>();
			if (permissionsManager == null)
				return;

			permissionsManager.RemoveObjectPermissions(this);
			permissionsManager.SetObjectPermissions(this, Permissions);
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

			ClearSettings();
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void DisposeFinal(bool disposing)
		{
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
			settings.Permissions = (Permissions ?? Enumerable.Empty<Permission>()).ToList();

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
			Permissions = (settings.Permissions ?? Enumerable.Empty<Permission>()).ToList();

			ResetPermissions();

			ApplySettingsFinal(settings, factory);

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

			Id = 0;
			Name = null;
			Permissions = Enumerable.Empty<Permission>();

			ResetPermissions();

			ClearSettingsFinal();

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
	}
}
