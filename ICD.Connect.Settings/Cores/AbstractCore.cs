using System;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCore<TSettings> : AbstractOriginator<TSettings>, ICore
		where TSettings : class, ICoreSettings, new()
	{
		private readonly CoreOriginatorCollection m_Originators;
		private readonly Localization.Localization m_Localization;

		#region Properties

		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public override string Category { get { return "Core"; } }

		/// <summary>
		/// Gets the originators contained in the core.
		/// </summary>
		public IOriginatorCollection<IOriginator> Originators { get { return m_Originators; } }

		/// <summary>
		/// Gets the configured localization.
		/// </summary>
		public Localization.Localization Localization { get { return m_Localization; } }

		/// <summary>
		/// Gets the datetime when this core was instantiated.
		/// </summary>
		public DateTime CoreStartTime { get; private set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractCore()
		{
			CoreStartTime = DateTime.UtcNow;
			m_Originators = new CoreOriginatorCollection();

			m_Localization = new Localization.Localization();
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
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			Localization.CopySettings(settings.LocalizationSettings);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Localization.ClearSettings();
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
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			// Setup localization first
			Localization.ApplySettings(settings.LocalizationSettings);
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
