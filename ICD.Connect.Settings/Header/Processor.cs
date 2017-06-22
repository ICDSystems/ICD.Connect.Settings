using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
#if SIMPLSHARP
using Crestron.SimplSharp;
#else
using System.Net;
#endif

namespace ICD.Connect.Settings.Header
{
	public sealed class Processor
	{
		public const string PROCESSOR_ELEMENT = "Processor";
		private const string MODEL_ELEMENT = "Model";
		private const string FIRMWARE_ELEMENT = "Firmware";
		private const string NETWORK_ADDRESS_ELEMENT = "NetworkAddress";

		private string Element { get { return PROCESSOR_ELEMENT; } }

		public string Model { get; set; }

		public Version Firmware { get; set; }

		public IPAddress NetworkAddress { get; set; }

		/// <summary>
		/// Creates a new Processor settings instance.
		/// Initializes the properties with default values
		/// </summary>
		public Processor() : this(false)
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
			if (currentSettings)
			{
#if SIMPLSHARP
				Model = CrestronUtils.ModelName;
				Firmware = CrestronUtils.ModelVersion;
#else
                Model = Environment.MachineName;
                Firmware = new Version("1.0.0.0");
#endif
				string address = IcdEnvironment.NetworkAddresses.FirstOrDefault() ?? "127.0.0.1";
				int i;
				NetworkAddress = new IPAddress(address.Split('.')
				                                      .Select(s => (byte)(StringUtils.TryParse(s, out i) ? i : 255))
				                                      .ToArray());
			}
			else
			{
				Model = "";
				Firmware = new Version("0.0.0.0");
				NetworkAddress = IPAddress.None;
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
			writer.WriteElementString(MODEL_ELEMENT, Model);
			writer.WriteElementString(FIRMWARE_ELEMENT, Firmware.ToString());
			writer.WriteElementString(NETWORK_ADDRESS_ELEMENT, NetworkAddress.ToString());
		}

		public static Processor ParseXml(string xml)
		{
			return new Processor
			{
				Model = GetModelFromXml(xml),
				Firmware = GetFirmwareFromXml(xml),
				NetworkAddress = GetNetworkAddressFromXml(xml)
			};
		}

		private static IPAddress GetNetworkAddressFromXml(string xml)
		{
			string ip = XmlUtils.TryReadChildElementContentAsString(xml, NETWORK_ADDRESS_ELEMENT);
			if (ip == null)
				return IPAddress.None;

			int i;
			return new IPAddress(ip.Split('.')
			                       .Select(s => (byte)(StringUtils.TryParse(s, out i) ? i : 255))
			                       .ToArray());
		}

		private static Version GetFirmwareFromXml(string xml)
		{
			return new Version(XmlUtils.TryReadChildElementContentAsString(xml, FIRMWARE_ELEMENT) ?? "0.0.0.0");
		}

		private static string GetModelFromXml(string xml)
		{
			return XmlUtils.TryReadChildElementContentAsString(xml, MODEL_ELEMENT) ?? "";
		}
	}
}
