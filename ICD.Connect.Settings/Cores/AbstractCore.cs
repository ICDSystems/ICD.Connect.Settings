using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Organizations;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCore<TSettings> : AbstractOriginator<TSettings>, ICore
		where TSettings : class, ICoreSettings, new()
	{
		private readonly Organization m_Organization;
		private readonly CoreOriginatorCollection m_Originators;
		private readonly Localization.Localization m_Localization;

		#region Properties

		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public override string Category { get { return "Core"; } }

		/// <summary>
		/// Gets/sets the organization info.
		/// </summary>
		public Organization Organization { get { return m_Organization; } }

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
			m_Organization = new Organization();
			m_Originators = new CoreOriginatorCollection();
			m_Localization = new Localization.Localization();

			CoreStartTime = DateTime.UtcNow;
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

			Organization.CopySettings(settings.OrganizationSettings);
			Localization.CopySettings(settings.LocalizationSettings);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Organization.ClearSettings();
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

			Organization.ApplySettings(settings.OrganizationSettings);
			Localization.ApplySettings(settings.LocalizationSettings);
		}

		/// <summary>
		/// Loads settings from disk and updates the Settings property.
		/// </summary>
		/// <param name="postApplyAction">Action that gets called after settings are applicd, before they are started</param>
		public void LoadSettings(Action postApplyAction)
		{
			FileOperations.LoadCoreSettings<AbstractCore<TSettings>, TSettings>(this, postApplyAction);
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in CoreConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			CoreConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in CoreConsole.GetConsoleCommands(this))
				yield return command;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
