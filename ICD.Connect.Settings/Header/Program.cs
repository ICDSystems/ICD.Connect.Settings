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
		private const string NAME_ELEMENT = "Name";
		private const string COMPILED_ON_ELEMENT = "CompiledOn";
		private const string VERSION_ELEMENT = "Version";

		#region Properties

		public string Name { get; private set; }

		public Version Version { get; private set; }

		public DateTime CompiledOn { get; private set; }

		#endregion

		/// <summary>
		/// Creates a new Program settings instance.
		/// Initializes the properties with default values
		/// </summary>
		public Program()
			: this(false)
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
			Clear();

			if (!currentSettings)
				return;

			Name = ProgramUtils.ApplicationName;
#if SIMPLSHARP
			Version = Assembly.GetExecutingAssembly().GetName().Version;
#else
			Version = Assembly.GetEntryAssembly().GetName().Version;
#endif
			CompiledOn = ProgramUtils.CompiledDate.ToUniversalTime();
		}

		#region Methods

		public void Clear()
		{
			Name = string.Empty;
			Version = new Version(0, 0);
			CompiledOn = DateTime.MinValue;
		}

		/// <summary>
		/// Writes the settings back to XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void ToXml(IcdXmlTextWriter writer, string element)
		{
			writer.WriteStartElement(element);
			{
				writer.WriteElementString(NAME_ELEMENT, Name);
				writer.WriteElementString(COMPILED_ON_ELEMENT, IcdXmlConvert.ToString(CompiledOn));
				writer.WriteElementString(VERSION_ELEMENT, Version.ToString());
			}
			writer.WriteEndElement();
		}

		public void ParseXml(string xml)
		{
			Clear();

			Name = XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT) ?? string.Empty;
			Version = new Version(XmlUtils.TryReadChildElementContentAsString(xml, VERSION_ELEMENT) ?? "0.0.0.0");
			CompiledOn = XmlUtils.TryReadChildElementContentAsDateTime(xml, COMPILED_ON_ELEMENT) ?? DateTime.MinValue;
		}

		#endregion
	}
}
