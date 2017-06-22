using System;
using ICD.Common.Attributes;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Attributes.Factories
{
	/// <summary>
	/// Base class for XmlFactoryMethod attributes.
	/// </summary>
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Method)]
	public abstract class AbstractXmlFactoryMethodAttribute : AbstractIcdAttribute
	{
		private readonly string m_TypeName;

		/// <summary>
		/// Returns the typename.
		/// </summary>
		[PublicAPI]
		public string TypeName { get { return m_TypeName; } }

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		protected AbstractXmlFactoryMethodAttribute(string typeName)
		{
			m_TypeName = typeName;
		}

		#endregion
	}
}
