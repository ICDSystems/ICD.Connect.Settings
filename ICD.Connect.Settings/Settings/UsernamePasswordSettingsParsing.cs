using System;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings
{
	public static class UsernamePasswordSettingsParsing
	{
		private const string USERNAME_ELEMENT = "Username";
		private const string PASSWORD_ELEMENT = "Password";

		/// <summary>
		/// Writes the username and password configuration to xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="instance"></param>
		public static void WriteElements(IcdXmlTextWriter writer, IUsernamePasswordSettings instance)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (instance == null)
				throw new ArgumentNullException("instance");

			writer.WriteElementString(USERNAME_ELEMENT, instance.Username);
			writer.WriteElementString(PASSWORD_ELEMENT, instance.Password);
		}

		/// <summary>
		/// Reads the username and password configuration from xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="instance"></param>
		public static void ParseXml(string xml, IUsernamePasswordSettings instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			instance.Username = XmlUtils.TryReadChildElementContentAsString(xml, USERNAME_ELEMENT);
			instance.Password = XmlUtils.TryReadChildElementContentAsString(xml, PASSWORD_ELEMENT);
		}
	}
}