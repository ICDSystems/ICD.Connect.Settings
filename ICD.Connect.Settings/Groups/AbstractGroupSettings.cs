using ICD.Common.Utils.Xml;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Connect.Settings.Groups
{
	public abstract class AbstractGroupSettings : AbstractSettings, IGroupSettings
	{
		private readonly List<int> m_Ids;

		public IEnumerable<int> Ids 
		{ 
			get { return m_Ids.ToList(); } 
			set
			{
				m_Ids.Clear();
				m_Ids.AddRange(value);
			}
		}
		
		protected abstract string RootElement { get; }
		protected abstract string ChildElement { get; }

		protected AbstractGroupSettings()
		{
			m_Ids = new List<int>();
		}

		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			XmlUtils.WriteListToXml(writer, Ids, RootElement, ChildElement);
		}

		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Ids = XmlUtils.ReadListFromXml(xml, RootElement, ChildElement, e => XmlUtils.ReadElementContentAsInt(e));
		}

		public override bool HasDependency(int id)
		{
			return m_Ids.Contains(id) || base.HasDependency(id);
		}
	}
}
