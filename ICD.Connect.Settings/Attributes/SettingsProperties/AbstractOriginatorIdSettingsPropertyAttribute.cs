using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	/// <summary>
	/// Describes an ID property that refers to a parent originator.
	/// </summary>
	public abstract class AbstractOriginatorIdSettingsPropertyAttribute : AbstractSettingsPropertyAttribute,
	                                                                      IOriginatorIdSettingsPropertyAttribute
	{
		private readonly Type m_OriginatorType;

		/// <summary>
		/// The type of originator to constrain against.
		/// </summary>
		[PublicAPI]
		public Type OriginatorType { get { return m_OriginatorType; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originatorType"></param>
		protected AbstractOriginatorIdSettingsPropertyAttribute(Type originatorType)
		{
			m_OriginatorType = originatorType;
		}
	}
}
