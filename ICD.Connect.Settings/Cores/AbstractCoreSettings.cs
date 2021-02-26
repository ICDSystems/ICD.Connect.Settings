using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Header;
using ICD.Connect.Settings.Localization;
using ICD.Connect.Settings.Organizations;
using ICD.Connect.Settings.Validation;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCoreSettings : AbstractSettings, ICoreSettings
	{
		private const string HEADER_ELEMENT = "Header";
		private const string ORGANIZATION_ELEMENT = "Organization";
		private const string LOCALIZATION_ELEMENT = "Localization";

		private readonly ConfigurationHeader m_Header;
		private readonly OrganizationSettings m_OrganizationSettings;
		private readonly LocalizationSettings m_LocalizationSettings;
		private readonly SettingsCollection m_OriginatorSettings;

		#region Properties

		/// <summary>
		/// Gets the header info.
		/// </summary>
		public ConfigurationHeader Header { get { return m_Header; } }

		/// <summary>
		/// Gets the organization settings.
		/// </summary>
		public OrganizationSettings OrganizationSettings { get { return m_OrganizationSettings; } }

		/// <summary>
		/// Gets the localization configuration.
		/// </summary>
		public LocalizationSettings LocalizationSettings { get { return m_LocalizationSettings; } }

		/// <summary>
		/// Gets the child originator settings collection.
		/// </summary>
		public SettingsCollection OriginatorSettings { get { return m_OriginatorSettings; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractCoreSettings()
		{
			m_Header = new ConfigurationHeader();
			m_OrganizationSettings = new OrganizationSettings();
			m_LocalizationSettings = new LocalizationSettings();
			m_OriginatorSettings = new SettingsCollection();

			m_OriginatorSettings.OnItemRemoved += OriginatorSettingsOnItemRemoved;
		}

		#region Methods

		/// <summary>
		/// Parses and returns only the header portion from the full XML config.
		/// </summary>
		/// <param name="configXml"></param>
		/// <returns></returns>
		public ConfigurationHeader GetHeader(string configXml)
		{
			ConfigurationHeader header = new ConfigurationHeader(false);
			UpdateHeaderFromXml(header, configXml);
			return header;
		}

		/// <summary>
		/// Validates the core settings as a whole.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SettingsValidationResult> Validate()
		{
			return Validate(this);
		}

		/// <summary>
		/// Validates this settings instance against the core settings as a whole.
		/// </summary>
		/// <param name="coreSettings"></param>
		/// <returns></returns>
		public override IEnumerable<SettingsValidationResult> Validate([NotNull] ICoreSettings coreSettings)
		{
			if (coreSettings == null)
				throw new ArgumentNullException("coreSettings");

			foreach (SettingsValidationResult result in BaseValidate(coreSettings))
				yield return result;

			foreach (SettingsValidationResult result in OriginatorSettings.SelectMany(s => s.Validate(coreSettings)))
				yield return result;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <param name="coreSettings"></param>
		/// <returns></returns>
		private IEnumerable<SettingsValidationResult> BaseValidate(ICoreSettings coreSettings)
		{
			return base.Validate(coreSettings);
		}

		/// <summary>
		/// Called when device settings are removed. We remove any settings that depend on the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected virtual void OriginatorSettingsOnItemRemoved(object sender, GenericEventArgs<ISettings> eventArgs)
		{
			// Remove dependencies from the core
			RemoveDependentSettings(m_OriginatorSettings, eventArgs.Data);
		}

		/// <summary>
		/// Removes settings from the collection that are dependent on the given settings instance.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="dependency"></param>
		protected static void RemoveDependentSettings(SettingsCollection collection, ISettings dependency)
		{
			ISettings[] remove = collection.Where(s => s.HasDependency(dependency.Id))
										   .ToArray();
			foreach (ISettings item in remove)
				collection.Remove(item);
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			new ConfigurationHeader(true).ToXml(writer, HEADER_ELEMENT);

			OrganizationSettings.ToXml(writer, ORGANIZATION_ELEMENT);
			LocalizationSettings.ToXml(writer, LOCALIZATION_ELEMENT);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			UpdateHeaderFromXml(m_Header, xml);
			UpdateOrganizationSettingsFromXml(m_OrganizationSettings, xml);
			UpdateLocalizationSettingsFromXml(m_LocalizationSettings, xml);
		}

		private static void UpdateHeaderFromXml(ConfigurationHeader header, string xml)
		{
			header.Clear();

			string child;
			if (XmlUtils.TryGetChildElementAsString(xml, HEADER_ELEMENT, out child))
				header.ParseXml(child);
		}

		private static void UpdateOrganizationSettingsFromXml(OrganizationSettings settings, string xml)
		{
			settings.Clear();

			string child;
			if (XmlUtils.TryGetChildElementAsString(xml, ORGANIZATION_ELEMENT, out child))
				settings.ParseXml(child);
		}

		private static void UpdateLocalizationSettingsFromXml(LocalizationSettings settings, string xml)
		{
			settings.Clear();

			string child;
			if (XmlUtils.TryGetChildElementAsString(xml, LOCALIZATION_ELEMENT, out child))
				settings.ParseXml(child);
		}

		#endregion
	}
}
