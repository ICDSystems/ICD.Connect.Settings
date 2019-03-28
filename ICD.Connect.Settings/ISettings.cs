using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Xml;

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
		/// Unique ID for the originator.
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// Custom name for the originator.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Custom name for the originator in a combined space.
		/// </summary>
		string CombineName { get; set; }

		/// <summary>
		/// Human readable text describing the originator.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Controls the visibility of the originator to the end user.
		/// Useful for hiding logical switchers, duplicate sources, etc.
		/// </summary>
		bool Hide { get; set; }

		/// <summary>
		/// The lookup name for the settings (typically the name of the originator).
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
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		void ParseXml(string xml);

		/// <summary>
		/// Writes the settings to xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		void ToXml(IcdXmlTextWriter writer, string element);

		/// <summary>
		/// Returns true if the settings depend on a device with the given ID.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		bool HasDependency(int id);

		#endregion
	}
}
