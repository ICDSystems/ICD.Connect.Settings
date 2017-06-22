using System;
using System.Globalization;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings.Header
{
	public sealed class ConfigurationHeader
	{
		private static readonly Version s_CurrentConfigVersion = new Version("1.1");

		public const string HEADER_ELEMENT = "Header";
		private const string CONFIG_VERSION_ELEMENT = "ConfigVersion";
		private const string GENERATED_ON_ELEMENT = "GeneratedOn";

		#region Properties

		private static string Element { get { return HEADER_ELEMENT; } }

		public Version ConfigVersion { get; private set; }

		public DateTime GeneratedOn { get; private set; }

		public Program Program { get; private set; }

		public Processor Processor { get; private set; }

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
			Program = new Program(currentSettings);
			Processor = new Processor(currentSettings);
		}

		/// <summary>
		/// Writes the settings back to XML.
		/// </summary>
		/// <param name="writer"></param>
		public void ToXml(IcdXmlTextWriter writer)
		{
			writer.WriteStartElement(Element);
			{
				WriteElements(writer);
			}
			writer.WriteEndElement();
		}

		private void WriteElements(IcdXmlTextWriter writer)
		{
			writer.WriteElementString(CONFIG_VERSION_ELEMENT, ConfigVersion.ToString());
			writer.WriteElementString(GENERATED_ON_ELEMENT, GeneratedOn.ToString("G"));

			Program.ToXml(writer);
			Processor.ToXml(writer);
		}

		public void Clear()
		{
			ConfigVersion = new Version(0, 0);
			GeneratedOn = DateTime.MinValue;
			Program = new Program(false);
			Processor = new Processor(false);
		}

		/// <summary>
		/// Parses the xml and returns a new ConfigurationHeader object
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			string programXml;
			string processorXml;
			Program program = XmlUtils.TryGetChildElementAsString(xml, Program.PROGRAM_ELEMENT, out programXml)
				                  ? Program.ParseXml(programXml)
				                  : new Program();
			Processor processor = XmlUtils.TryGetChildElementAsString(xml, Processor.PROCESSOR_ELEMENT, out processorXml)
				                      ? Processor.ParseXml(processorXml)
				                      : new Processor();

			ConfigVersion = GetConfigVersionFromXml(xml);
			GeneratedOn = GetGeneratedOnFromXml(xml);
			Program = program;
			Processor = processor;
		}

		private static Version GetConfigVersionFromXml(string xml)
		{
			return new Version(XmlUtils.TryReadChildElementContentAsString(xml, CONFIG_VERSION_ELEMENT) ?? "0.0.0.0");
		}

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
	}
}
