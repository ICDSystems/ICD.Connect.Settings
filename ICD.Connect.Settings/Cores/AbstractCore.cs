using System;
using ICD.Connect.Settings.Originators;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Connect.Telemetry.Service;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCore<TSettings> : AbstractOriginator<TSettings>, ICore
		where TSettings : class, ICoreSettings, new()
	{
		private readonly CoreOriginatorCollection m_Originators;
		private readonly Localization.Localization m_Localization;

		#region Properties

		/// <summary>
		/// Gets the originators contained in the core.
		/// </summary>
		public IOriginatorCollection<IOriginator> Originators { get { return m_Originators; } }

		/// <summary>
		/// Gets the configured localization.
		/// </summary>
		public Localization.Localization Localization { get { return m_Localization; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractCore()
		{
			m_Originators = new CoreOriginatorCollection();
			m_Originators.OnOriginatorAdded += OriginatorsOnOriginatorAdded;
			m_Originators.OnOriginatorRemoved += OriginatorsOnOriginatorRemoved;

			m_Localization = new Localization.Localization();

			ITelemetryService telemetry = ServiceProvider.TryGetService<ITelemetryService>();
			if (telemetry != null)
				telemetry.AddTelemetryProvider(this);
		}

		#region Originator Collection Callbacks

		/// <summary>
		/// Called when an originator is added to the collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void OriginatorsOnOriginatorAdded(object sender, GenericEventArgs<IOriginator> args)
		{
			ITelemetryService telemetry = ServiceProvider.TryGetService<ITelemetryService>();
			if (telemetry != null)
				telemetry.AddTelemetryProvider(args.Data);
		}

		/// <summary>
		/// Called when an originator is removed from the collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void OriginatorsOnOriginatorRemoved(object sender, GenericEventArgs<IOriginator> args)
		{
			ITelemetryService telemetry = ServiceProvider.TryGetService<ITelemetryService>();
			if (telemetry != null)
				telemetry.RemoveTelemetryProvider(args.Data);
		}

		#endregion

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
