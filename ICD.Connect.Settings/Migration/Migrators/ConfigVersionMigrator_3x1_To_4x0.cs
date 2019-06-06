using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.Settings.Utils;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronXmlLinq;
#else
using System.Xml.Linq;
#endif

namespace ICD.Connect.Settings.Migration.Migrators
{
	/// <summary>
	/// Moved dialers out of DialingPlan, added ConferencePoints.
	/// </summary>
	public sealed class ConfigVersionMigrator_3x1_To_4x0 : AbstractConfigVersionMigrator
	{
		/// <summary>
		/// Gets the starting version for the input configuration.
		/// </summary>
		public override Version From { get { return new Version(3, 1); } }

		/// <summary>
		/// Gets the resulting version for the output configuration.
		/// </summary>
		public override Version To { get { return new Version(4, 0); } }

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

			XElement conferencePoints = new XElement("ConferencePoints");
			root.Add(conferencePoints);

			XElement rooms = root.Element("Rooms");
			IEnumerable<XElement> roomElements = rooms == null ? Enumerable.Empty<XElement>() : rooms.Elements();

			foreach (XElement room in roomElements)
				MigrateRoom(root, room, conferencePoints);

			return document.ToString();
		}

		private void MigrateRoom(XElement root, XElement room, XElement conferencePoints)
		{
			/*
			<DialingPlan>
				<Config>DialingPlan.xml</Config>
				<AudioEndpoint>
					<Device>200002</Device>
					<Control>25</Control>
				</AudioEndpoint>
				<VideoEndpoint>
					<Device>202002</Device>
					<Control>1</Control>
				</VideoEndpoint>				
			</DialingPlan>
			*/

			// Becomes

			/*
			<DialingPlan>DialingPlan.xml</DialingPlan>
			*/

			XElement dialingPlan = room.Element("DialingPlan");

			if (dialingPlan == null)
			{
				dialingPlan = new XElement("DialingPlan");
				room.Add(dialingPlan);
			}

			// Create the conference points
            XElement audioEndpoint = dialingPlan.Element("AudioEndpoint");
			XElement audioDeviceElement = audioEndpoint == null ? null : audioEndpoint.Element("Device");
			XElement audioControlElement = audioEndpoint == null ? null : audioEndpoint.Element("Control");

			int audioDeviceId = 0;
			if (audioDeviceElement != null)
				StringUtils.TryParse(audioDeviceElement.Value, out audioDeviceId);

			int audioControlId = 0;
			if (audioControlElement != null)
				StringUtils.TryParse(audioControlElement.Value, out audioControlId);

			XElement videoEndpoint = dialingPlan.Element("VideoEndpoint");
			XElement videoDeviceElement = videoEndpoint == null ? null : videoEndpoint.Element("Device");
			XElement videoControlElement = videoEndpoint == null ? null : videoEndpoint.Element("Control");

			int videoDeviceId = 0;
			if (videoDeviceElement != null)
				StringUtils.TryParse(videoDeviceElement.Value, out videoDeviceId);

			int videoControlId = 0;
			if (videoControlElement != null)
				StringUtils.TryParse(videoControlElement.Value, out videoControlId);

			int roomId = 0;
			XAttribute roomIdAttribute = room.Attribute("id");
			if (roomIdAttribute != null)
				StringUtils.TryParse(roomIdAttribute.Value, out roomId);

			List<int> ids = new List<int>();

			if (audioDeviceId == videoDeviceId &&
			    audioControlId == videoControlId &&
			    audioDeviceId != 0)
			{
				// Audio and video endpoints are the same
				int conferencePointId;
				AddConferencePoint(root, conferencePoints, roomId, audioDeviceId, audioControlId, "Audio, Video",
				                   out conferencePointId);
				ids.Add(conferencePointId);
			}
			else
			{
				if (audioDeviceId != 0)
				{
					int conferencePointId;
					AddConferencePoint(root, conferencePoints, roomId, audioDeviceId, audioControlId, "Audio", out conferencePointId);
					ids.Add(conferencePointId);
				}

				if (videoDeviceId != 0)
				{
					int conferencePointId;
					AddConferencePoint(root, conferencePoints, roomId, videoDeviceId, videoControlId, "Video", out conferencePointId);
					ids.Add(conferencePointId);
				}
			}

			// Add the conference point ids to the room
			XElement conferencePointIds = new XElement("ConferencePoints");
			room.Add(conferencePointIds);

			foreach (int id in ids)
			{
				XElement idElement = new XElement("ConferencePoint", id);
				conferencePointIds.Add(idElement);
			}

			// Migrate the dialing plan
			XElement config = dialingPlan.Element("Config");
			string configPath = config == null ? null : config.Value;

			dialingPlan.RemoveNodes();
			if (configPath != null)
				dialingPlan.SetValue(configPath);
		}

		private void AddConferencePoint(XElement root, XElement conferencePoints, int roomId, int deviceId, int controlId,
		                                string type, out int id)
		{
			// Get all of the ids in the document
			IEnumerable<int> ids = RecursionUtils.BreadthFirstSearch(root, e => e.Elements())
			                                     .Select(e => e.Attribute("id"))
			                                     .Where(a => a != null)
			                                     .Select(a => int.Parse(a.Value));

			id = IdUtils.GetNewId(ids, IdUtils.SUBSYSTEM_POINTS, roomId);

			// Build the conference point
			XElement conferencePoint = new XElement("ConferencePoint");
			conferencePoint.Add(new XAttribute("id", id));
			conferencePoint.Add(new XAttribute("type", "ConferencePoint"));
			conferencePoint.Add(new XElement("Device", deviceId));
			conferencePoint.Add(new XElement("Control", controlId));
			conferencePoint.Add(new XElement("Type", type));

			// Add the conference point
			conferencePoints.Add(conferencePoint);
		}
	}
}
