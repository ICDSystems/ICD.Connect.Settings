namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlPortSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlPortSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
