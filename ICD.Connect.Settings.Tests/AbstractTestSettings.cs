using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.Originators;
using ICD.Connect.Settings.Validation;

namespace ICD.Connect.Settings.Tests
{
    public abstract class AbstractTestSettings : ISettings
    {
	    public event EventHandler<IntEventArgs> OnIdChanged;

	    public event EventHandler<StringEventArgs> OnNameChanged;

	    public int Id { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    /// <summary>
	    /// Unique ID for the originator.
	    /// </summary>
	    public Guid NewId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    public string Name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

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

	    public bool Disable { get; set; }
	    public int Order { get; set; }

	    public string FactoryName { get { throw new NotImplementedException(); } }

	    public Type OriginatorType { get { throw new NotImplementedException(); } }

	    public IEnumerable<Permission> Permissions { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    public void ParseXml(string xml)
	    {
		    throw new NotImplementedException();
	    }

	    /// <summary>
	    /// Writes the settings to xml.
	    /// </summary>
	    /// <param name="writer"></param>
	    /// <param name="element"></param>
	    public void ToXml(IcdXmlTextWriter writer, string element)
	    {
		    throw new NotImplementedException();
	    }

	    public IOriginator ToOriginator(IDeviceFactory factory)
	    {
		    throw new NotImplementedException();
	    }

	    /// <summary>
	    /// Returns true if the settings depend on a device with the given ID.
	    /// For example, to instantiate an IR Port from settings, the device the physical port
	    /// belongs to will need to be instantiated first.
	    /// </summary>
	    /// <returns></returns>
	    public bool HasDependency(int id)
	    {
		    throw new NotImplementedException();
	    }

	    /// <summary>
	    /// Validates this settings instance against the core settings as a whole.
	    /// </summary>
	    /// <param name="coreSettings"></param>
	    /// <returns></returns>
	    public IEnumerable<SettingsValidationResult> Validate(ICoreSettings coreSettings)
	    {
			throw new NotImplementedException();
		}
    }
}
