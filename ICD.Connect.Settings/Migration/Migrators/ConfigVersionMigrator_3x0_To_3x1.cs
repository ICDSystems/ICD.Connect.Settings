using System;
using System.Linq;
using ICD.Common.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXmlLinq;
#else
using System.Xml.Linq;
#endif

namespace ICD.Connect.Settings.Migration.Migrators
{
	/// <summary>
	/// Ports now have configuration items for networking, comspec, URIs, etc
	/// </summary>
	public sealed class ConfigVersionMigrator_3x0_To_3x1 : AbstractConfigVersionMigrator
	{
		/// <summary>
		/// Gets the starting version for the input configuration.
		/// </summary>
		public override Version From { get { return new Version(3, 0); } }

		/// <summary>
		/// Gets the resulting version for the output configuration.
		/// </summary>
		public override Version To { get { return new Version(3, 1); } }

		/// <summary>
		/// Migrates the input xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override string Migrate(string xml)
		{
			XDocument document = XDocument.Parse(xml);
			XElement root = document.Root;
			if (root == null)
				throw new FormatException();

			foreach (XElement element in root.Elements("Ports"))
				MigratePorts(element);

			/*
			foreach (XElement element in root.Elements("Devices"))
				MigrateDevices(element);
			 */

			return document.ToString();
		}

		#region Ports

		private void MigratePorts(XElement portsElement)
		{
			foreach (XElement element in portsElement.Elements())
				MigratePort(element);
		}

		private void MigratePort(XElement portElement)
		{
			XAttribute typeAttribute = portElement.Attribute("type");
			if (typeAttribute == null)
				return;

			switch (typeAttribute.Value)
			{
				case "HTTP":
					MigrateWebPortElement(portElement);
					break;

				case "IrPort":
				case "MockIrPort":
					MigrateIrPortElement(portElement);
					break;

				case "TCP":
				case "SSH":
				case "UDP":
					MigrateNetworkPortElement(portElement);
					break;
			}
		}

		private void MigrateWebPortElement(XElement portElement)
		{
			/*
			<Port id="101002" type="HTTP">
			  <Name>Clickshare Control</Name>
			  <CombineName />
			  <Description />
			  <Address>https://10.58.88.215:4001/</Address>
			  <Username>integrator</Username>
			  <Password>xhTg3CUAv5Y2</Password>
			  <Accept>application/json</Accept>
			  <Hide>false</Hide>
			</Port>
			*/

			// Becomes

			/*
			<Port id="101002" type="HTTP">
			  <Name>Clickshare Control</Name>
			  <CombineName />
			  <Description />
			  <Uri>
				<Scheme>https</Scheme>
				<Username>integrator</Username>
				<Password>xhTg3CUAv5Y2</Password>
				<Host>10.58.88.215</Host>
				<Port>4001</Port>
				<Path>/</Path>
				<Query />
				<Fragment />
			  </Uri>
			  <Hide>false</Hide>
			</Port>
			*/

			XElement[] nodes = portElement.Descendants().ToArray();
			portElement.RemoveNodes();

			XElement uriElement = new XElement("Uri");

			foreach (XElement node in nodes)
			{
				// ReSharper disable RedundantToStringCall
				switch (node.Name.ToString())
				// ReSharper restore RedundantToStringCall
				{
					// Accept was deprecated
					case "Accept":
						break;

					case "Username":
					case "Password":
						uriElement.Add(node);
						break;

					case "Address":
						IcdUriBuilder uriBuilder =
							string.IsNullOrEmpty(node.Value)
								? new IcdUriBuilder()
								: new IcdUriBuilder(node.Value);

						XElement scheme = new XElement("Scheme");
						if (uriBuilder.Scheme != null)
							scheme.Value = uriBuilder.Scheme;

						XElement host = new XElement("Host");
						if (uriBuilder.Host != null)
							host.Value = uriBuilder.Host;

						XElement port = new XElement("Port");
						if (uriBuilder.Port != 0)
							port.Value = uriBuilder.Port.ToString();

						XElement path = new XElement("Path");
						if (uriBuilder.Path != null)
							path.Value = uriBuilder.Path;

						XElement query = new XElement("Query");
						if (uriBuilder.Query != null)
							query.Value = uriBuilder.Query;

						XElement fragment = new XElement("Fragment");
						if (uriBuilder.Fragment != null)
							fragment.Value = uriBuilder.Fragment;

						uriElement.Add(scheme);
						uriElement.Add(host);
						uriElement.Add(port);
						uriElement.Add(path);
						uriElement.Add(query);
						uriElement.Add(fragment);

						break;

					default:
						portElement.Add(node);
						break;
				}
			}

			portElement.Add(uriElement);
		}

		private void MigrateIrPortElement(XElement portElement)
		{
			/*
			<Port id="101003" type="IrPort">
			  <Name>DMPS3 to TV Tuner IR</Name>
			  <CombineName />
			  <Description />
			  <Device>201000</Device>
			  <Address>1</Address>
			  <Driver>Amino H140 RSDICD Standard.ir</Driver>
			  <PulseTime>100</PulseTime>
			  <BetweenTime>750</BetweenTime>
			  <Hide>false</Hide>
			</Port>
			*/

			// Becomes

			/*
			<Port id="101003" type="IrPort">
			  <Name>DMPS3 to TV Tuner IR</Name>
			  <CombineName />
			  <Description />
			  <IR>
				<Driver>Amino H140 RSDICD Standard.ir</Driver>
				<PulseTime>100</PulseTime>
				<BetweenTime>750</BetweenTime>
			  </IR>
			  <Device>201000</Device>
			  <Address>1</Address>
			  <Hide>false</Hide>
			</Port>
			*/

			XElement[] nodes = portElement.Descendants().ToArray();
			portElement.RemoveNodes();

			XElement irElement = new XElement("IR");

			foreach (XElement node in nodes)
			{
				// ReSharper disable RedundantToStringCall
				switch (node.Name.ToString())
				// ReSharper restore RedundantToStringCall
				{
					case "Driver":
					case "PulseTime":
					case "BetweenTime":
						irElement.Add(node);
						break;

					default:
						portElement.Add(node);
						break;
				}
			}

			portElement.Add(irElement);
		}

		private void MigrateNetworkPortElement(XElement portElement)
		{
			/*
			<Port id="101000" type="SSH">
			  <Name>SSH port for Codec</Name>
			  <CombineName />
			  <Description />
			  <Address>10.58.96.21</Address>
			  <Port>22</Port>
			  <Username>crestron</Username>
			  <Password>NFUPcPxs5p2XeKn6W6Qn</Password>
			  <Hide>false</Hide>
			</Port>
			*/

			// Becomes

			/*
			<Port id="101000" type="SSH">
			  <Name>SSH port for Codec</Name>
			  <CombineName />
			  <Description />
			  <Network>
				<Address>10.58.96.21</Address>
				<Port>22</Port>
				<Username>crestron</Username>
				<Password>NFUPcPxs5p2XeKn6W6Qn</Password>
			  </Network>
			  <Hide>false</Hide>
			</Port>
			*/

			XElement[] nodes = portElement.Descendants().ToArray();
			portElement.RemoveNodes();

			XElement networkElement = new XElement("Network");

			foreach (XElement node in nodes)
			{
// ReSharper disable RedundantToStringCall
				switch (node.Name.ToString())
// ReSharper restore RedundantToStringCall
				{
					case "Address":
					case "Port":
					case "Username":
					case "Password":
						networkElement.Add(node);
						break;

					default:
						portElement.Add(node);
						break;
				}
			}

			portElement.Add(networkElement);
		}

		#endregion
	}
}
