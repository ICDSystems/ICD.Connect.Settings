using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings.Header
{
	public sealed class Processor
	{
		private const string MODEL_ELEMENT = "Model";
		private const string FIRMWARE_ELEMENT = "Firmware";
		private const string NETWORK_ADDRESS_ELEMENT = "NetworkAddress";
		private const string MAC_ADDRESS_ELEMENT = "MacAddress";

		#region Properties

		public string Model { get; set; }

		public Version Firmware { get; set; }

		public string NetworkAddress { get; set; }

		public string MacAddress { get; set; }

		#endregion

		/// <summary>
		/// Creates a new Processor settings instance.
		/// Initializes the properties with default values
		/// </summary>
		public Processor()
			: this(false)
		{
		}

		/// <summary>
		/// Creates a new Processor settings instance. If currentSettings is true, 
		/// initializes the properties with values from the currently running program.
		/// Otherwise, it uses default values as if no configuration were present.
		/// </summary>
		/// <param name="currentSettings">true to initialize with new settings, false for default/minimum values</param>
		public Processor(bool currentSettings)
		{
			Clear();

			if (!currentSettings)
				return;

			Model = ProcessorUtils.ModelName;
			Firmware = ProcessorUtils.ModelVersion;
			NetworkAddress = IcdEnvironment.NetworkAddresses.FirstOrDefault() ?? "127.0.0.1";
			MacAddress = IcdEnvironment.MacAddresses.FirstOrDefault();
		}

		#region Methods

		public void Clear()
		{
			Model = string.Empty;
			Firmware = new Version(0, 0);
			NetworkAddress = string.Empty;
			MacAddress = string.Empty;
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
				writer.WriteElementString(MODEL_ELEMENT, Model);
				writer.WriteElementString(FIRMWARE_ELEMENT, Firmware.ToString());
				writer.WriteElementString(NETWORK_ADDRESS_ELEMENT, NetworkAddress);
				writer.WriteElementString(MAC_ADDRESS_ELEMENT, MacAddress);
			}
			writer.WriteEndElement();
		}

		public void ParseXml(string xml)
		{
			Clear();

			Model = XmlUtils.TryReadChildElementContentAsString(xml, MODEL_ELEMENT) ?? string.Empty;
			Firmware = new Version(XmlUtils.TryReadChildElementContentAsString(xml, FIRMWARE_ELEMENT) ?? "0.0.0.0");
			NetworkAddress = XmlUtils.TryReadChildElementContentAsString(xml, NETWORK_ADDRESS_ELEMENT);
			MacAddress = XmlUtils.TryReadChildElementContentAsString(xml, MAC_ADDRESS_ELEMENT);
		}

		#endregion
	}
}
