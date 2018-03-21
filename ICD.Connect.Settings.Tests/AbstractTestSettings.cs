using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings.Tests
{
    public abstract class AbstractTestSettings : ISettings
    {
	    public event EventHandler<IntEventArgs> OnIdChanged;

	    public event EventHandler<StringEventArgs> OnNameChanged;

	    public int Id { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    public string Name { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    public string CombineName { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    public string FactoryName { get { throw new NotImplementedException(); } }

	    public Type OriginatorType { get { throw new NotImplementedException(); } }

	    public IEnumerable<Permission> Permissions { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

	    public int DependencyCount { get { throw new NotImplementedException(); } }

	    public void ParseXml(string xml)
	    {
		    throw new NotImplementedException();
	    }

	    public void ToXml(IcdXmlTextWriter writer)
	    {
		    throw new NotImplementedException();
	    }

	    public IOriginator ToOriginator(IDeviceFactory factory)
	    {
		    throw new NotImplementedException();
	    }

	    public bool HasDeviceDependency(int id)
	    {
		    throw new NotImplementedException();
	    }
    }
}
