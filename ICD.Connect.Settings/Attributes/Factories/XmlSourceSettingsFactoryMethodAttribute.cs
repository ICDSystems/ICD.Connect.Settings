namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlSourceSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlSourceSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
