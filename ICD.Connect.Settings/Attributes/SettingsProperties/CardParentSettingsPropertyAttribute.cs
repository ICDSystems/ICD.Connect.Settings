using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	/// <summary>
	/// Describes a settings property that refers to a parent card cage, DM frame, etc.
	/// </summary>
	[MeansImplicitUse]
	public sealed class CardParentSettingsPropertyAttribute : AbstractOriginatorIdSettingsPropertyAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="originatorType"></param>
		public CardParentSettingsPropertyAttribute(Type originatorType)
			: base(originatorType)
		{
		}
	}
}
