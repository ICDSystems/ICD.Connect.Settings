using System;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings
{
	public abstract class AbstractUsernamePasswordProperties : IUsernamePasswordProperties
	{
		private const string USERNAME_ELEMENT = "Username";
		private const string PASSWORD_ELEMENT = "Password";

		/// <summary>
		/// Gets/sets the configurable username.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Gets/sets the configurable password.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Writes the username and password configuration to xml.
		/// </summary>
		/// <param name="writer"></param>
		public virtual void WriteElements(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteElementString(USERNAME_ELEMENT, Username);
			writer.WriteElementString(PASSWORD_ELEMENT, Password);
		}

		/// <summary>
		/// Reads the username and password configuration from xml.
		/// </summary>
		/// <param name="xml"></param>
		public virtual void ParseXml(string xml)
		{
			Username = XmlUtils.TryReadChildElementContentAsString(xml, USERNAME_ELEMENT);
			Password = XmlUtils.TryReadChildElementContentAsString(xml, PASSWORD_ELEMENT);
		}
	}
}
