using System;
using System.Globalization;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings.Header
{
	public sealed class ConfigurationHeader
	{
		private static readonly Version s_CurrentConfigVersion = new Version("3.1");

		private const string CONFIG_VERSION_ELEMENT = "ConfigVersion";
		private const string GENERATED_ON_ELEMENT = "GeneratedOn";

		private const string PROGRAM_ELEMENT = "Program";
		private const string PROCESSOR_ELEMENT = "Processor";

		private readonly Program m_Program;
		private readonly Processor m_Processor;

		#region Properties

		/// <summary>
		/// Gets the current config version.
		/// </summary>
		public static Version CurrentConfigVersion { get { return s_CurrentConfigVersion; } }

		/// <summary>
		/// Gets the config version.
		/// </summary>
		public Version ConfigVersion { get; private set; }

		/// <summary>
		/// Gets the date that the config was generated on.
		/// </summary>
		public DateTime GeneratedOn { get; private set; }

		/// <summary>
		/// Gets the header program information.
		/// </summary>
		public Program Program { get { return m_Program; } }

		/// <summary>
		/// Gets the header processor information.
		/// </summary>
		public Processor Processor { get { return m_Processor; } }

		#endregion

		/// <summary>
		/// Creates a new ConfigurationHeader settings instance.
		/// Initializes the properties with default values
		/// </summary>
		public ConfigurationHeader()
			: this(false)
		{
		}

		/// <summary>
		/// Creates a new Program settings instance. If currentSettings is true, 
		/// initializes the properties with values from the currently running program.
		/// Otherwise, it uses default values as if no configuration were present.
		/// </summary>
		/// <param name="currentSettings">true to initialize with new settings, false for default/minimum values</param>
		public ConfigurationHeader(bool currentSettings)
		{
			if (currentSettings)
			{
				ConfigVersion = s_CurrentConfigVersion;
				GeneratedOn = IcdEnvironment.GetLocalTime();
			}
			else
			{
				ConfigVersion = new Version(0, 0);
				GeneratedOn = DateTime.MinValue;
			}

			m_Program = new Program(currentSettings);
			m_Processor = new Processor(currentSettings);
		}

		#region Methods

		/// <summary>
		/// Resets the configuration header to default values.
		/// </summary>
		public void Clear()
		{
			ConfigVersion = new Version(0, 0);
			GeneratedOn = DateTime.MinValue;

			m_Program.Clear();
			m_Processor.Clear();
		}

		/// <summary>
		/// Writes the configuration header to XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		public void ToXml(IcdXmlTextWriter writer, string elementName)
		{
			writer.WriteStartElement(elementName);
			{
				writer.WriteElementString(CONFIG_VERSION_ELEMENT, ConfigVersion.ToString());
				writer.WriteElementString(GENERATED_ON_ELEMENT, GeneratedOn.ToString("G"));

				m_Program.ToXml(writer, PROGRAM_ELEMENT);
				m_Processor.ToXml(writer, PROCESSOR_ELEMENT);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Reads the configuration header from xml.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Clear();

			ConfigVersion = new Version(XmlUtils.TryReadChildElementContentAsString(xml, CONFIG_VERSION_ELEMENT) ?? "0.0.0.0");
			GeneratedOn = GetGeneratedOnFromXml(xml);

			string programXml;
			XmlUtils.TryGetChildElementAsString(xml, PROGRAM_ELEMENT, out programXml);
			if (!string.IsNullOrEmpty(programXml))
				m_Program.ParseXml(programXml);

			string processorXml;
			XmlUtils.TryGetChildElementAsString(xml, PROCESSOR_ELEMENT, out processorXml);
			if (!string.IsNullOrEmpty(processorXml))
				m_Processor.ParseXml(processorXml);
		}

		#endregion

		#region Private Methods

		private static DateTime GetGeneratedOnFromXml(string xml)
		{
			string date = XmlUtils.TryReadChildElementContentAsString(xml, GENERATED_ON_ELEMENT);
			if (string.IsNullOrEmpty(date))
				return DateTime.MinValue;

			try
			{
				DateTime.ParseExact(date, "G", CultureInfo.CurrentCulture);
			}
			catch (FormatException)
			{
				return DateTime.MinValue;
			}

			return DateTime.MinValue;
		}

		#endregion
	}
}
