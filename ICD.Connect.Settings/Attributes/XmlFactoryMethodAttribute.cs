using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Attributes;

namespace ICD.Connect.Settings.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class XmlFactoryMethodAttribute : AbstractIcdAttribute
	{
		private readonly string m_FactoryName;

		/// <summary>
		/// Returns the typename.
		/// </summary>
		[PublicAPI]
		public string FactoryName { get { return m_FactoryName; } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName"></param>
		public XmlFactoryMethodAttribute(string factoryName)
		{
			m_FactoryName = factoryName;
		}

		#endregion
	}
}
