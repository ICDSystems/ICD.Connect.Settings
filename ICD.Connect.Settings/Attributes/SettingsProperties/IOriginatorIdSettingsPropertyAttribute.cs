using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	/// <summary>
	/// Describes an ID property that refers to a parent originator.
	/// </summary>
	public interface IOriginatorIdSettingsPropertyAttribute : ISettingsPropertyAttribute
	{
		/// <summary>
		/// The type of originator to constrain against.
		/// </summary>
		[PublicAPI]
		Type OriginatorType { get; }
	}
}
