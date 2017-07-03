using System;
using System.Collections.Generic;
using ICD.Common.EventArguments;
using ICD.Common.Permissions;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// ISettings represents a simple instance that can be stored as XML.
	/// </summary>
	public interface ISettings
	{
		#region Events

		/// <summary>
		/// Raised when the id is changed.
		/// </summary>
		[PublicAPI]
		event EventHandler<IntEventArgs> OnIdChanged;

		/// <summary>
		/// Raised when the name is changed.
		/// </summary>
		[PublicAPI]
		event EventHandler<StringEventArgs> OnNameChanged;

		#endregion

		#region Properties

		/// <summary>
		/// Unique ID for the settings.
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// Custom name for the settings.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// The name of the factory (typically the name of the originator).
		/// </summary>
		string FactoryName { get; }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		Type OriginatorType { get; }

		/// <summary>
		/// Gets the list of permissions
		/// </summary>
		IEnumerable<Permission> Permissions { get; set; } 

		#endregion

		#region Methods

		/// <summary>
		/// Writes the settings to xml.
		/// </summary>
		/// <param name="writer"></param>
		void ToXml(IcdXmlTextWriter writer);

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		IOriginator ToOriginator(IDeviceFactory factory);

		/// <summary>
		/// Returns the collection of ids that the settings will depend on.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		IEnumerable<int> GetDeviceDependencies();

		#endregion
	}
}
