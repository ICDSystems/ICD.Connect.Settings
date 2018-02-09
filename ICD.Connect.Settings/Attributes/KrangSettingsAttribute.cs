using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Attributes;

namespace ICD.Connect.Settings.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class KrangSettingsAttribute : AbstractIcdAttribute
	{
		private readonly string m_FactoryName;

		/// <summary>
		/// Returns the typename.
		/// </summary>
		[PublicAPI]
		public string FactoryName { get { return m_FactoryName; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName"></param>
		public KrangSettingsAttribute(string factoryName)
		{
			m_FactoryName = factoryName;
		}
	}
}
