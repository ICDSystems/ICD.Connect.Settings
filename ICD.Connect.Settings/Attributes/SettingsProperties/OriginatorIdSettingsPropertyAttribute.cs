using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	[MeansImplicitUse]
	public sealed class OriginatorIdSettingsPropertyAttribute : AbstractSettingsPropertyAttribute
	{
		private readonly Type m_OriginatorType;

		/// <summary>
		/// In the case of an Id property, describes the type of originator to constrain against.
		/// </summary>
		[PublicAPI]
		public Type OriginatorType { get { return m_OriginatorType; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originatorType"></param>
		public OriginatorIdSettingsPropertyAttribute(Type originatorType)
		{
			m_OriginatorType = originatorType;
		}
	}
}
