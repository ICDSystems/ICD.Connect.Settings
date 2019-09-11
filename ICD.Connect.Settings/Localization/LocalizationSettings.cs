using System;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings.Localization
{
	public sealed class LocalizationSettings
	{
		private const string ELEMENT_CULTURE = "Culture";
		private const string ELEMENT_UI_CULTURE = "UiCulture";
		private const string ELEMENT_OVERRIDE_24_HOUR = "Override24Hour";

		#region Properties

		/// <summary>
		/// Gets/sets the culture name.
		/// </summary>
		public string Culture { get; set; }

		/// <summary>
		/// Gets/sets the UI culture name.
		/// </summary>
		public string UiCulture { get; set; }

		/// <summary>
		/// Gets/sets the 24 hour override mode.
		/// </summary>
		public Localization.e24HourOverride Override24Hour { get; set; }

		#endregion

		#region Methods

		public void Clear()
		{
			Culture = null;
			UiCulture = null;
			Override24Hour = Localization.e24HourOverride.None;
		}

		/// <summary>
		/// Updates the settings from the given xml element.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			Culture = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_CULTURE);
			UiCulture = XmlUtils.TryReadChildElementContentAsString(xml, ELEMENT_UI_CULTURE);

			Override24Hour =
				XmlUtils.TryReadChildElementContentAsEnum<Localization.e24HourOverride>(xml, ELEMENT_OVERRIDE_24_HOUR, true) ??
				Localization.e24HourOverride.None;
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
				writer.WriteElementString(ELEMENT_CULTURE, IcdXmlConvert.ToString(Culture));
				writer.WriteElementString(ELEMENT_UI_CULTURE, IcdXmlConvert.ToString(UiCulture));
				writer.WriteElementString(ELEMENT_OVERRIDE_24_HOUR, IcdXmlConvert.ToString(Override24Hour));
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
