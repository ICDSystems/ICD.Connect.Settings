using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
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
		private const string VERSION_ATTRIBUTE = "assemblyVersion";

		public event EventHandler<IntEventArgs> OnIdChanged;
		public event EventHandler<StringEventArgs> OnNameChanged;

		private string m_Name;
		private int m_Id;
		private Version m_AssemblyVersion;

		protected string AssemblyVersion
		{
			get
			{
				return GetType()
#if SIMPLSHARP
					.GetCType()
#else
                    .GetTypeInfo()
#endif
					.Assembly
					.GetName()
					.Version
					.ToString();
			}
		}

		#region Properties

		/// <summary>
		/// Unique ID for the settings.
		/// </summary>
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
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(String.Format("{0} failed to create instance of {1}", GetType().Name, OriginatorType.Name), e);
			}

			output.ApplySettings(this, factory);
			return output;
		}

		/// <summary>
		/// Returns the collection of ids that the settings will depend on.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<int> GetDeviceDependencies();

		/// <summary>
		/// Returns the value of the id attribute from the given xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static int GetIdFromXml(string xml)
		{
			return XmlUtils.GetAttributeAsInt(xml, ID_ATTRIBUTE);
		}

		private static Version GetVersionFromXml(string xml)
		{
			if (XmlUtils.HasAttribute(xml, VERSION_ATTRIBUTE))
			{
				string version = XmlUtils.GetAttributeAsString(xml, VERSION_ATTRIBUTE);
				if (!string.IsNullOrEmpty(version))
					return new Version(version);
			}

			return new Version(1, 0);
		}

		/// <summary>
		/// Gets the name from the xml element.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		private static string GetNameFromXml(string xml)
		{
			return XmlUtils.TryReadChildElementContentAsString(xml, NAME_ELEMENT);
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
			WriteVersionAttribute(writer);
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
		/// Writes the version attribute to xml.
		/// </summary>
		/// <param name="writer"></param>
		private void WriteVersionAttribute(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteAttributeString(VERSION_ATTRIBUTE, AssemblyVersion);
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

			instance.Id = GetIdFromXml(xml);
			instance.Name = GetNameFromXml(xml);
			instance.m_AssemblyVersion = GetVersionFromXml(xml);
		}

		#endregion
	}
}
