using System;
using ICD.Common.Utils.Xml;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Connect.Settings.Groups
{
	public abstract class AbstractGroupSettings : AbstractSettings, IGroupSettings
	{
		private readonly List<int> m_Ids;

		#region Properties

		/// <summary>
		/// Override to determine the name of the items array root element.
		/// </summary>
		protected abstract string RootElement { get; }

		/// <summary>
		/// Override to determine the name of the items array child elements.
		/// </summary>
		protected abstract string ChildElement { get; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractGroupSettings()
		{
			m_Ids = new List<int>();
		}

		#region Methods

		/// <summary>
		/// Sets the ids for the child items.
		/// </summary>
		public void SetIds(IEnumerable<int> ids)
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			m_Ids.Clear();
			m_Ids.AddRange(ids.Distinct());
		}

		/// <summary>
		/// Gets the ids for the child items.
		/// </summary>
		public IEnumerable<int> GetIds()
		{
			return m_Ids.ToArray();
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			XmlUtils.WriteListToXml(writer, GetIds(), RootElement, ChildElement);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			SetIds(XmlUtils.ReadListFromXml(xml, RootElement, ChildElement, e => XmlUtils.ReadElementContentAsInt(e)));
		}

		#endregion

		/// <summary>
		/// Returns true if the settings depend on an originator with the given ID.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public override bool HasDependency(int id)
		{
			return m_Ids.Contains(id) || base.HasDependency(id);
		}
	}
}
