using System;
using System.Collections.Generic;
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
		/// Returns true if this instance has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Logging service for all your logging needs
		/// </summary>
		protected ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

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
			List<string> properties = new List<string>();
			BuildStringRepresentationProperties((name, value) => properties.Add(string.Format("{0}={1}", name, value)));

			string propertiesString = string.Join(", ", properties.ToArray());
			return string.Format("{0}({1})", GetType().Name, propertiesString);
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

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases resources but also allows for finalizing without touching managed resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
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
			OnSettingsClearing = null;
			OnSettingsCleared = null;
			OnSettingsApplied = null;
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
