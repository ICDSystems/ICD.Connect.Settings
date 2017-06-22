using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif

namespace ICD.Connect.Settings.Header
{
	public sealed class Program
	{
		public const string PROGRAM_ELEMENT = "Program";
		private const string NAME_ELEMENT = "Name";
		private const string COMPILED_ON_ELEMENT = "CompiledOn";
		private const string VERSION_ELEMENT = "Version";

		private string Element { get { return PROGRAM_ELEMENT; } }

		public string Name { get; private set; }

		public Version Version { get; private set; }

		public string CompiledOn { get; private set; }

		/// <summary>
		/// Creates a new Program settings instance.
		/// Initializes the properties with default values
		/// </summary>
		public Program() : this(false)
		{
		}

		/// <summary>
		/// Creates a new Program settings instance. If currentSettings is true, 
		/// initializes the properties with values from the currently running program.
		/// Otherwise, it uses default values as if no configuration were present.
		/// </summary>
		/// <param name="currentSettings">true to initialize with new settings, false for default/minimum values</param>
		public Program(bool currentSettings)
		{
			if (currentSettings)
			{
				Name = ProgramUtils.ApplicationName;
#if SIMPLSHARP
				Version = Assembly.GetExecutingAssembly().GetName().Version;
#else
                Version = Assembly.GetEntryAssembly().GetName().Version;
#endif
				CompiledOn = ProgramUtils.CompiledDate;
			}
			else
			{
				Name = string.Empty;
				Version = new Version("0.0.0.0");
				CompiledOn = string.Empty;
			}
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
			writer.WriteElementString(NAME_ELEMENT, Name);
			writer.WriteElementString(COMPILED_ON_ELEMENT, CompiledOn);
			writer.WriteElementString(VERSION_ELEMENT, Version.ToString());
		}

		public static Program ParseXml(string xml)
		{
			return new Program
			{
				Name = GetNameFromXml(xml),
				Version = GetVersionFromXml(xml),
				CompiledOn = GetCompiledOnFromXml(xml)
			};
		}

		private static string GetCompiledOnFromXml(string xml)
		{
			return XmlUtils.TryReadChildElementContentAsString(xml, COMPILED_ON_ELEMENT);
		}

		private static Version GetVersionFromXml(string xml)
		{
			return new Version(XmlUtils.TryReadChildElementContentAsString(xml, VERSION_ELEMENT) ?? "0.0.0.0");
		}

		private static string GetNameFromXml(string xml)
		{
			return XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT) ?? "";
		}
	}
}
