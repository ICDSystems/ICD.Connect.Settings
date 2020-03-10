using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXmlLinq;
#else
using System.Xml.Linq;
#endif
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;

namespace ICD.Connect.Settings.Migration.Migrators
{
	/// <summary>
	/// Moves VolumePoints out of the MetlifeRoom and into the Krang config.
	/// Sources and Destinations now have multiple addresses.
	/// </summary>
	public sealed class ConfigVersionMigrator_2x0_To_3x0 : AbstractConfigVersionMigrator
	{
		private const string ID_REGEX = @"id=\""(?'id'\d+)\""";

		private const string VOLUMEPOINT_REGEX = @"<VolumePoint>(?'elements'.*?)<\/VolumePoint>";

		/// <summary>
		/// Gets the starting version for the input configuration.
		/// </summary>
		public override Version From { get { return new Version(2, 0); } }

		/// <summary>
		/// Gets the resulting version for the output configuration.
		/// </summary>
		public override Version To { get { return new Version(3, 0); } }

		/// <summary>
		/// Migrates the input xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override string Migrate(string xml)
		{
			xml = MigrateVolumePoints(xml);
			xml = MigrateSourcesDestinations(xml);

			return xml;
		}

		/// <summary>
		/// Moves MetlifeVolumePoints out of the room and into the VolumePoints element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static string MigrateVolumePoints(string xml)
		{
			// Build a set of unique ids in the xml
			IcdHashSet<int> ids = new IcdHashSet<int>();
			foreach (Match match in Regex.Matches(xml, ID_REGEX))
			{
				int id;
				if (StringUtils.TryParse(match.Groups["id"].Value, out id))
					ids.Add(id);
			}

			Dictionary<int, string> oldVolumePoints = new Dictionary<int, string>();

			// Replace the existing old-style volume points
			string xmlCopy = xml;
			Func<Match, string> replacement =
				match =>
				{
					// Make a new id and keep track of the volume point contents
					int newId = IdUtils.GetNewId(ids, eSubsystems.VolumePoints);

					string elements = match.Groups["elements"].Value;
					elements = FixDisplayVolumeControlId(xmlCopy, elements);

					ids.Add(newId);
					oldVolumePoints.Add(newId, elements);

					return newId.ToString();
				};

			xml = RegexUtils.ReplaceGroup(xml, VOLUMEPOINT_REGEX, "elements", replacement, RegexOptions.Singleline);

			// Append the new-style volume points
			xml = xml.Replace("</IcdConfig>", "");

			StringBuilder builder = new StringBuilder(xml);
			{
				builder.Append("<VolumePoints>");
				{
					foreach (KeyValuePair<int, string> kvp in oldVolumePoints)
					{
						builder.AppendFormat("<VolumePoint id=\"{0}\" type=\"MetlifeVolumePoint\">", kvp.Key);
						{
							builder.Append(kvp.Value);
						}
						builder.Append("</VolumePoint>");
					}
				}
				builder.Append("</VolumePoints>");

				builder.Append("</IcdConfig>");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Display volume control ids changed from 1 to 2.
		/// </summary>
		/// <param name="configXml"></param>
		/// <param name="volumePointElements"></param>
		/// <returns></returns>
		private static string FixDisplayVolumeControlId(string configXml, string volumePointElements)
		{
			const string controlIdRegex = @"<Control>(?'id'\d+)<\/Control>";
			const string deviceIdRegex = @"<Device>(?'id'\d+)<\/Device>";
			const string deviceAttributesRegex = @"<Device\s+id=""{0}""\s+type=""(?'type'[^""]+)"">";

			// Get the control id
			Match controlMatch;
			if (!RegexUtils.Matches(volumePointElements, controlIdRegex, out controlMatch))
				return volumePointElements;

			int controlId = int.Parse(controlMatch.Groups["id"].Value);
			if (controlId != 1)
				return volumePointElements;

			// Get the device id
			Match deviceMatch;
			if (!RegexUtils.Matches(volumePointElements, deviceIdRegex, out deviceMatch))
				return volumePointElements;

			int deviceId = int.Parse(deviceMatch.Groups["id"].Value);

			// Is the device a display?
			string regex = string.Format(deviceAttributesRegex, deviceId);
			Match deviceTypeMatch;
			if (!RegexUtils.Matches(configXml, regex, out deviceTypeMatch))
				return volumePointElements;

			string deviceType = deviceTypeMatch.Groups["type"].Value;
			if (!DeviceTypeIsDisplay(deviceType))
				return volumePointElements;

			// Replace the control id
			return RegexUtils.ReplaceGroup(volumePointElements, controlIdRegex, "id", "2");
		}

		private static bool DeviceTypeIsDisplay(string deviceType)
		{
			// Lucky, this actually covers everything!
			return deviceType.Contains("Display");
		}

		/// <summary>
		/// Moves Source/Destination addresses into a child Addresses element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static string MigrateSourcesDestinations(string xml)
		{
			XDocument document = XDocument.Parse(xml);

			XElement root = document.Root;
			if (root == null)
				throw new FormatException();

			IEnumerable<XElement> destinationNodes = root.Elements("Routing")
			                                             .SelectMany(r => r.Elements("Destinations"))
			                                             .SelectMany(d => d.Elements("Destination"));

			Dictionary<XElement, IcdHashSet<XElement>> destinationMatches = GetMatchingDestinations(destinationNodes);

			foreach (KeyValuePair<XElement, IcdHashSet<XElement>> kvp in destinationMatches)
			{
				IcdHashSet<string> addresses = GetAddressesForDestinations(kvp.Value.Prepend(kvp.Key));

				foreach (XElement destination in kvp.Value)
					destination.Remove();

				MigrateAddressToAddressesElement(kvp.Key);

				AddAddressesToDestination(kvp.Key, addresses);
			}

			IEnumerable<XElement> removedElements = destinationMatches.Values.SelectMany(d => d);
			IcdHashSet<string> removedIds = new IcdHashSet<string>();
			foreach (XElement element in removedElements)
			{
				XAttribute attribute = element.Attribute("id");
				if (attribute == null)
					throw new FormatException("No id attribute found on destination element. Cannot migrate an invalid configuration");

				if (string.IsNullOrEmpty(attribute.Value))
					throw new FormatException("Invalid id attribute on destination element. Cannot migrate an invalid configuration");

				removedIds.Add(attribute.Value);
			}

			RemoveDestinationsFromDevices(root, removedIds);

			return document.ToString();
		}

		private static void AddAddressesToDestination(XElement element, IEnumerable<string> addresses)
		{
			XElement addressesElement = element.Elements("Addresses").FirstOrDefault();
			if (addressesElement == null)
				throw new InvalidOperationException();

			foreach (string address in addresses)
				addressesElement.Add(new XElement("Address", address));
		}

		private static void RemoveDestinationsFromDevices(XElement root, IcdHashSet<string> destinationIds)
		{
			IEnumerable<XElement> roomDestinations = root.Elements("Rooms")
			                                             .SelectMany(r => r.Elements("Room"))
			                                             .SelectMany(r => r.Elements("Destinations"))
			                                             .SelectMany(r => r.Elements("Destination"));

			foreach (XElement destination in roomDestinations)
			{
				if (destinationIds.Contains(destination.Value))
					destination.Remove();
			}
		}

		private static void MigrateAddressToAddressesElement(XElement element)
		{
			XElement addressElement = element.Elements("Address").FirstOrDefault();
			if (addressElement != null)
				addressElement.Remove();

			XElement addressesElement = element.Elements("Addresses").FirstOrDefault();
			if (addressesElement == null)
			{
				addressesElement = new XElement("Addresses");
				element.Add(addressesElement);
			}
		}

		private static IcdHashSet<string> GetAddressesForDestinations(IEnumerable<XElement> elements)
		{
			IcdHashSet<string> addresses = new IcdHashSet<string>();

			foreach (XElement value in elements)
				addresses.AddRange(GetAllAddressesForDestination(value));

			return addresses;
		}

		private static IcdHashSet<string> GetAllAddressesForDestination(XElement dest)
		{
			IcdHashSet<string> addresses = new IcdHashSet<string>();
			XElement addressElement = dest.Elements("Address").FirstOrDefault();
			if (addressElement != null)
				addresses.Add(addressElement.Value);

			XElement addressesElement = dest.Elements("Addresses").FirstOrDefault();
			if (addressesElement != null)
			{
				foreach (XElement childAddress in addressesElement.Elements("Address"))
					addresses.Add(childAddress.Value);
			}

			return addresses;
		}

		private static Dictionary<XElement, IcdHashSet<XElement>> GetMatchingDestinations(
			IEnumerable<XElement> destinationNodes)
		{
			Dictionary<XElement, IcdHashSet<XElement>> destinationMatches = new Dictionary<XElement, IcdHashSet<XElement>>();
			IcdHashSet<XElement> processedDestinations = new IcdHashSet<XElement>();

			foreach (XElement destination in destinationNodes)
			{
				if (processedDestinations.Contains(destination))
					continue;

				foreach (XElement potentialMatch in destinationMatches.Keys)
				{
					if (!CheckDestinationTypeMatch(destination, potentialMatch))
						continue;

					if (!CheckDestinationDeviceMatch(destination, potentialMatch))
						continue;

					if (!CheckDestinationControlMatch(destination, potentialMatch))
						continue;

					destinationMatches[potentialMatch].Add(destination);
					processedDestinations.Add(destination);
					break;
				}

				if (processedDestinations.Contains(destination))
					continue;

				destinationMatches.Add(destination, new IcdHashSet<XElement>());
				processedDestinations.Add(destination);
			}

			return destinationMatches;
		}

		private static bool CheckDestinationTypeMatch(XElement destination, XElement potentialMatch)
		{
			XAttribute destinationTypeAttribute = destination.Attribute("type");
			if (destinationTypeAttribute == null)
				return false;

			XAttribute potentialMatchTypeAttribute = potentialMatch.Attribute("type");
			if (potentialMatchTypeAttribute == null)
				return false;

			return destinationTypeAttribute.Value == potentialMatchTypeAttribute.Value;
		}

		private static bool CheckDestinationDeviceMatch(XElement destination, XElement potentialMatch)
		{
			XElement destinationDeviceNode = destination.Elements("Device").FirstOrDefault();
			if (destinationDeviceNode == null)
				return false;

			XElement potentialMatchDeviceNode = potentialMatch.Elements("Device").FirstOrDefault();
			if (potentialMatchDeviceNode == null)
				return false;

			return destinationDeviceNode.Value == potentialMatchDeviceNode.Value;
		}

		private static bool CheckDestinationControlMatch(XElement destination, XElement potentialMatch)
		{
			XElement destinationControlNode = destination.Elements("Control").FirstOrDefault();
			if (destinationControlNode == null)
				return false;

			XElement potentialMatchControlNode = potentialMatch.Elements("Control").FirstOrDefault();
			if (potentialMatchControlNode == null)
				return false;

			return destinationControlNode.Value == potentialMatchControlNode.Value;
		}
	}
}
