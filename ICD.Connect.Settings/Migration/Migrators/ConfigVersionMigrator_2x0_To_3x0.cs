using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

		private const string ADDRESS_REGEX =
			@"<(Source|Destination)(.*)(?'addressElement'<Address>(?'address'\d+)<\/Address>)(.*)<\/(Source|Destination)>";

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
					int newId = IdUtils.GetNewId(ids, IdUtils.SUBSYSTEM_POINTS, 0);

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
			Func<Match, string> replacement =
				match =>
				{
					int address;
					return StringUtils.TryParse(match.Groups["address"].Value, out address)
						? string.Format("<Addresses><Address>{0}</Address></Addresses>", address)
						: string.Empty;
				};

			return RegexUtils.ReplaceGroup(xml, ADDRESS_REGEX, "addressElement", replacement, RegexOptions.Singleline);
		}
	}
}
