using System.Collections.Generic;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings.Services
{
	public abstract class AbstractServiceSettings : AbstractSettings, IServiceSettings
	{
		private const string PROVIDERS_ELEMENT = "Providers";
		private const string PROVIDER_ELEMENT = "Provider";

		private readonly SettingsCollection m_ProviderSettings;

		/// <summary>
		/// Gets the provider settings for the service.
		/// </summary>
		public SettingsCollection ProviderSettings { get { return m_ProviderSettings; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractServiceSettings()
		{
			m_ProviderSettings = new SettingsCollection();
		}

		#region Methods

		/// <summary>
		/// Writes the routing settings to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			m_ProviderSettings.ToXml(writer, PROVIDERS_ELEMENT, PROVIDER_ELEMENT);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			IEnumerable<ISettings> providers = PluginFactory.GetSettingsFromXml(xml, PROVIDERS_ELEMENT);

			AddSettingsLogDuplicates(ProviderSettings, providers);
		}

		private void AddSettingsLogDuplicates(SettingsCollection collection, IEnumerable<ISettings> settings)
		{
			foreach (ISettings item in settings)
			{
				if (collection.Add(item))
					continue;

				ServiceProvider.GetService<ILoggerService>()
							   .AddEntry(eSeverity.Error, "{0} failed to add duplicate {1}", GetType().Name, item);
			}
		}

		#endregion
	}
}
