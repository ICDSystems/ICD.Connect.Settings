using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes
{
	/// <summary>
	/// Provides information on a settings property.
	/// </summary>
	[PublicAPI]
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public sealed class SettingsProperty : Attribute
	{
		public enum ePropertyType
		{
			[PublicAPI] Default,
			[PublicAPI] Hidden,
			[PublicAPI] Id,
			[PublicAPI] Ipid
		}

		private readonly ePropertyType m_PropertyType;
		private readonly Type m_Type;

		/// <summary>
		/// Gets the property type.
		/// </summary>
		[PublicAPI]
		public ePropertyType PropertyType { get { return m_PropertyType; } }

		/// <summary>
		/// In the case of an Id property, describes the type of originator to constrain against.
		/// </summary>
		[PublicAPI]
		public Type Type { get { return m_Type; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyType"></param>
		public SettingsProperty(ePropertyType propertyType)
			: this(propertyType, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyType"></param>
		/// <param name="type"></param>
		public SettingsProperty(ePropertyType propertyType, Type type)
		{
			m_PropertyType = propertyType;
			m_Type = type;
		}
	}
}
