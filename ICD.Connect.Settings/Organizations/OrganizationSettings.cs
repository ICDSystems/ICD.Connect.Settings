using System;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings.Organizations
{
	public sealed class OrganizationSettings
	{
		private const string ELEMENT_ID = "Id";
		private const string ELEMENT_NAME = "Name";

		#region Properties

		/// <summary>
		/// Gets/sets the ID of the organization.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets/sets the name of the organization.
		/// </summary>
		public string Name { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Reverts the settings to defaults.
		/// </summary>
		public void Clear()
		{
			Id = 0;
			Name = null;
		}

		/// <summary>
		/// Updates the settings from the given xml element.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Id = XmlUtils.TryReadChildElementContentAsInt(xml, ELEMENT_ID) ?? 0;
			Name = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_NAME);
		}

		/// <summary>
		/// Writes the current configuration to the given XML writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void ToXml(IcdXmlTextWriter writer, string element)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement(element);
			{
				writer.WriteElementString(ELEMENT_ID, IcdXmlConvert.ToString(Id));
				writer.WriteElementString(ELEMENT_NAME, Name);
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
