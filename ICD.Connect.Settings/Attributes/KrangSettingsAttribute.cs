using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Attributes;

namespace ICD.Connect.Settings.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class KrangSettingsAttribute : AbstractIcdAttribute
	{
		private readonly string m_FactoryName;
		private readonly Type m_OriginatorType;

		/// <summary>
		/// Returns the registered factory name.
		/// </summary>
		[PublicAPI]
		public string FactoryName { get { return m_FactoryName; } }

		/// <summary>
		/// Returns the type of originator to instantiate.
		/// </summary>
		[PublicAPI]
		public Type OriginatorType { get { return m_OriginatorType; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName"></param>
		[Obsolete("Provide originator type to constructor")]
		public KrangSettingsAttribute(string factoryName)
			: this(factoryName, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName"></param>
		/// <param name="originatorType"></param>
		public KrangSettingsAttribute(string factoryName, Type originatorType)
		{
			m_FactoryName = factoryName;
			m_OriginatorType = originatorType;
		}
	}
}
