using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Permissions;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.Validation;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Base class for all settings objects.
	/// </summary>
	public abstract class AbstractSettings : ISettings
	{
		public const string TYPE_ATTRIBUTE = "type";
		private const string ID_ATTRIBUTE = "id";
		private const string UUID_ATTRIBUTE = "uuid";

		private const string NAME_ELEMENT = "Name";
		private const string COMBINE_NAME_ELEMENT = "CombineName";
		private const string HIDE_ELEMENT = "Hide";
		private const string DISABLE_ELEMENT = "Disable";
		private const string ORDER_ELEMENT = "Order";
		private const string DESCRIPTION_ELEMENT = "Description";

		private const string PERMISSION_ELEMENT = "Permission";
		private const string PERMISSIONS_ELEMENT = PERMISSION_ELEMENT + "s";

		/// <summary>
		/// Raised when the name is changed.
		/// </summary>
		public event EventHandler<StringEventArgs> OnNameChanged;

		private string m_Name;
		private int m_Order = int.MaxValue;

		#region Properties

		/// <summary>
		/// Unique ID for the settings.
		/// </summary>
		[HiddenSettingsProperty]
		public int Id { get; set; }

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		[HiddenSettingsProperty]
		public Guid Uuid { get; set; }

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
		/// Shorthand for disabling an instance in the system.
		/// </summary>
		public bool Disable { get; set; }

		/// <summary>
		/// Specifies custom ordering of the instance to the end user.
		/// </summary>
		public int Order { get { return m_Order; } set { m_Order = value; } }

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

		protected ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractSettings()
		{
			Permissions = Enumerable.Empty<Permission>();
		}

		#region Methods

		/// <summary>
		/// Gets the string representation for this instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);

			if (Id != 0)
				builder.AppendProperty("Id", Id);

			if (!string.IsNullOrEmpty(Name) && Name != GetType().Name)
			{
				builder.AppendProperty("Name", Name);

				if (!string.IsNullOrEmpty(CombineName) && CombineName != Name)
					builder.AppendProperty("CombineName", CombineName);
			}

			return builder.ToString();
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public virtual void ParseXml(string xml)
		{
			Id = XmlUtils.GetAttributeAsInt(xml, ID_ATTRIBUTE);
			Uuid = XmlUtils.GetAttributeAsGuid(xml, UUID_ATTRIBUTE);
			Name = XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT);
			CombineName = XmlUtils.TryReadChildElementContentAsString(xml, COMBINE_NAME_ELEMENT);
			Description = XmlUtils.TryReadChildElementContentAsString(xml, DESCRIPTION_ELEMENT);
			Hide = XmlUtils.TryReadChildElementContentAsBoolean(xml, HIDE_ELEMENT) ?? false;
			Disable = XmlUtils.TryReadChildElementContentAsBoolean(xml, DISABLE_ELEMENT) ?? false;
			Order = XmlUtils.TryReadChildElementContentAsInt(xml, ORDER_ELEMENT) ?? 0;
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
			writer.WriteAttributeString(UUID_ATTRIBUTE, IcdXmlConvert.ToString(Uuid));
			writer.WriteAttributeString(TYPE_ATTRIBUTE, FactoryName);
			{
				writer.WriteElementString(NAME_ELEMENT, Name);
				writer.WriteElementString(COMBINE_NAME_ELEMENT, CombineName);
				writer.WriteElementString(DESCRIPTION_ELEMENT, Description);
				
				WriteElements(writer);

				writer.WriteElementString(HIDE_ELEMENT, IcdXmlConvert.ToString(Hide));

				if (Disable)
					writer.WriteElementString(DISABLE_ELEMENT, IcdXmlConvert.ToString(Disable));

				if (Order != int.MaxValue)
					writer.WriteElementString(ORDER_ELEMENT, IcdXmlConvert.ToString(Order));

				XmlUtils.WriteListToXml(writer, Permissions, PERMISSIONS_ELEMENT, (w, p) => p.ToXml(w, PERMISSION_ELEMENT));
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
		/// Returns true if the settings depend on an originator with the given ID.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public virtual bool HasDependency(int id)
		{
			return AttributeUtils.GetProperties<OriginatorIdSettingsPropertyAttribute>(this, true)
			                     .Any(p => p.GetValue(this, null) as int? == id);
		}

		/// <summary>
		/// Validates this settings instance against the core settings as a whole.
		/// </summary>
		/// <param name="coreSettings"></param>
		/// <returns></returns>
		public virtual IEnumerable<SettingsValidationResult> Validate(ICoreSettings coreSettings)
		{
			yield break;
		}

		#endregion
	}
}
