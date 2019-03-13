using System;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCore<TSettings> : AbstractOriginator<TSettings>, ICore
		where TSettings : class, ICoreSettings, new()
	{
		private readonly CoreOriginatorCollection m_Originators;

		/// <summary>
		/// Gets the originators contained in the core.
		/// </summary>
		public IOriginatorCollection<IOriginator> Originators { get { return m_Originators; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractCore()
		{
			m_Originators = new CoreOriginatorCollection();
		}

		#region Settings

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		void ICore.CopySettings(ICoreSettings settings)
		{
			CopySettings((TSettings)settings);
		}

		/// <summary>
		/// Copies the current state of the Core instance.
		/// </summary>
		/// <returns></returns>
		ICoreSettings ICore.CopySettings()
		{
			return CopySettings();
		}

		/// <summary>
		/// Applies the settings to the Core instance.
		/// </summary>
		/// <param name="settings"></param>
		void ICore.ApplySettings(ICoreSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			IDeviceFactory factory = new CoreDeviceFactory(settings);
			ApplySettings((TSettings)settings, factory);
		}

		/// <summary>
		/// Loads settings from disk and updates the Settings property.
		/// </summary>
		public void LoadSettings()
		{
			FileOperations.LoadCoreSettings<AbstractCore<TSettings>, TSettings>(this);
		}

		#endregion
	}
}