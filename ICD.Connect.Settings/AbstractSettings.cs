using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;
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
		private const string HIDE_ELEMENT = "Hide";
		private const string DESCRIPTION_ELEMENT = "Description";

		protected ILoggerService Logger {get { return ServiceProvider.TryGetService<ILoggerService>(); }}

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
		/// Human readable text describing the originator.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Controls the visibility of the originator to the end user.
		/// Useful for hiding logical switchers, duplicate sources, etc.
		/// </summary>
		public bool Hide { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public virtual string FactoryName
		{
			get
			{
				KrangSettingsAttribute attribute = AttributeUtils.GetClassAttribute<KrangSettingsAttribute>(GetType());
				if (attribute == null)
					throw new InvalidOperationException(string.Format("{0} has no FactoryName", GetType().Name));

				return attribute.FactoryName;
			}
		}

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public virtual Type OriginatorType
		{
			get
			{
				KrangSettingsAttribute attribute = AttributeUtils.GetClassAttribute<KrangSettingsAttribute>(GetType());
				if (attribute == null)
					throw new InvalidOperationException(string.Format("{0} has no OriginatorType", GetType().Name));

				return attribute.OriginatorType;
			}
		}

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

		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);

			if (Id != 0)
				builder.AppendProperty("Id", Id);

			if (!string.IsNullOrEmpty(Name) && Name != GetType().Name)
				builder.AppendProperty("Name", Name);

			return builder.ToString();
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public virtual void ParseXml(string xml)
		{
			Id = XmlUtils.GetAttributeAsInt(xml, ID_ATTRIBUTE);
			Name = XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT);
			CombineName = XmlUtils.TryReadChildElementContentAsString(xml, COMBINE_NAME_ELEMENT);
			Description = XmlUtils.TryReadChildElementContentAsString(xml, DESCRIPTION_ELEMENT);
			Hide = XmlUtils.TryReadChildElementContentAsBoolean(xml, HIDE_ELEMENT) ?? false;
			Permissions = XmlUtils.ReadListFromXml(xml, PERMISSIONS_ELEMENT, PERMISSION_ELEMENT, e => Permission.FromXml(e));
		}

		/// <summary>
		/// Writes the settings back to XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void ToXml(IcdXmlTextWriter writer, string element)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement(element);
			writer.WriteAttributeString(ID_ATTRIBUTE, Id.ToString());
			writer.WriteAttributeString(TYPE_ATTRIBUTE, FactoryName);
			{
				writer.WriteElementString(NAME_ELEMENT, Name);
				writer.WriteElementString(COMBINE_NAME_ELEMENT, CombineName);
				writer.WriteElementString(DESCRIPTION_ELEMENT, Description);
				
				WriteElements(writer);

				writer.WriteElementString(HIDE_ELEMENT, IcdXmlConvert.ToString(Hide));
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
			{
				throw new InvalidOperationException(string.Format("{0} is not assignable to {1}", OriginatorType.Name,
				                                                  typeof(IOriginator).Name));
			}

			IOriginator output;

			try
			{
				output = (IOriginator)ReflectionUtils.CreateInstance(OriginatorType);

				// This instance came from settings, so we want to store it back to settings.
				output.Serialize = true;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(
					string.Format("{0} failed to create instance of {1} - Error Message: {2} Inner Message:{3}", GetType().Name,
					              OriginatorType.Name, e.Message, e.InnerException), e);
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

		#endregion
	}
}
