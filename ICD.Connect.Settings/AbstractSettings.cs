using System;
using System.Collections.Generic;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes.SettingsProperties;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Base class for all settings objects.
	/// </summary>
	public abstract class AbstractSettings : ISettings
	{
		public const string ID_ATTRIBUTE = "id";
		public const string TYPE_ATTRIBUTE = "type";
		public const string NAME_ELEMENT = "Name";
		public const string COMBINE_NAME_ELEMENT = "CombineName";

		private const string PERMISSION_ELEMENT = "Permission";
		private const string PERMISSIONS_ELEMENT = PERMISSION_ELEMENT + "s";

		public event EventHandler<IntEventArgs> OnIdChanged;
		public event EventHandler<StringEventArgs> OnNameChanged;

		private string m_Name;
		private int m_Id;

		#region Properties

		/// <summary>
		/// Unique ID for the settings.
		/// </summary>
		[HiddenSettingsProperty]
		public int Id
		{
			get { return m_Id; }
			set
			{
				if (value == m_Id)
					return;

				m_Id = value;

				OnIdChanged.Raise(this, new IntEventArgs(m_Id));
			}
		}

		/// <summary>
		/// Custom name for the settings.
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set
			{
				if (value == m_Name)
					return;

				m_Name = value;

				OnNameChanged.Raise(this, new StringEventArgs(m_Name));
			}
		}

		/// <summary>
		/// Custom name for the originator in a combined space.
		/// </summary>
		public string CombineName { get; set; }

		/// <summary>
		/// Gets the xml element.
		/// </summary>
		protected abstract string Element { get; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public abstract string FactoryName { get; }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public abstract Type OriginatorType { get; }

		/// <summary>
		/// Gets the list of permissions
		/// </summary>
		[HiddenSettingsProperty]
		public IEnumerable<Permission> Permissions { get; set; }

        /// <summary>
        /// Returns the count from the collection of ids that the settings depends on.
        /// </summary>
        public virtual int DependencyCount { get { return 0; } }

		#endregion

		#region Methods

	    /// <summary>
		/// Writes the settings back to XML.
		/// </summary>
		/// <param name="writer"></param>
		public void ToXml(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			WriteStartElement(writer);
			{
				WriteNameElement(writer);
				WriteElements(writer);
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected virtual void WriteElements(IcdXmlTextWriter writer)
		{
		}

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IOriginator ToOriginator(IDeviceFactory factory)
		{
			if (!OriginatorType.IsAssignableTo(typeof(IOriginator)))
				throw new InvalidOperationException(string.Format("{0} is not assignable to {1}", OriginatorType.Name, typeof(IOriginator).Name));

			IOriginator output;

			try
			{
				output = (IOriginator)ReflectionUtils.CreateInstance(OriginatorType);
				
				// This instance came from settings, so we want to store it back to settings.
				output.Serialize = true;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(String.Format("{0} failed to create instance of {1} - Error Message: {2} Inner Message:{3}", GetType().Name, OriginatorType.Name, e.Message, e.InnerException), e);
			}

			output.ApplySettings(this, factory);
			return output;
		}

	    /// <summary>
	    /// Returns true if the settings depend on a device with the given ID.
	    /// For example, to instantiate an IR Port from settings, the device the physical port
	    /// belongs to will need to be instantiated first.
	    /// </summary>
	    /// <returns></returns>
	    public virtual bool HasDeviceDependency(int id)
	    {
	        return false;
	    }

	    /// <summary>
		/// Gets the set of permissions from the xml element
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static IEnumerable<Permission> GetPermissionsFromXml(string xml)
		{
			string permissionsElement;
			if (XmlUtils.TryGetChildElementAsString(xml, PERMISSIONS_ELEMENT, out permissionsElement))
			{
				foreach (var permission in XmlUtils.GetChildElementsAsString(permissionsElement, PERMISSION_ELEMENT))
					yield return Permission.FromXml(permission);
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Writes the start element to xml.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteStartElement(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement(Element);
			WriteIdAttribute(writer);
			WriteTypeAttribute(writer);
		}

		/// <summary>
		/// Writes the id attribute to xml.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteIdAttribute(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteAttributeString(ID_ATTRIBUTE, Id.ToString());
		}

		/// <summary>
		/// Writes the type element to xml.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteTypeAttribute(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteAttributeString(TYPE_ATTRIBUTE, FactoryName);
		}

		/// <summary>
		/// Writes the name element to xml.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteNameElement(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteElementString(NAME_ELEMENT, Name);
			writer.WriteElementString(COMBINE_NAME_ELEMENT, CombineName);
		}

		/// <summary>
		/// Parses the xml and applies the properties to the instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="xml"></param>
		protected static void ParseXml(AbstractSettings instance, string xml)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			instance.Id = XmlUtils.GetAttributeAsInt(xml, ID_ATTRIBUTE);
			instance.Name = XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT);
			instance.CombineName = XmlUtils.TryReadChildElementContentAsString(xml, COMBINE_NAME_ELEMENT);
			instance.Permissions = GetPermissionsFromXml(xml);
		}

		#endregion
	}
}
