namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlStaticRouteSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlStaticRouteSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
