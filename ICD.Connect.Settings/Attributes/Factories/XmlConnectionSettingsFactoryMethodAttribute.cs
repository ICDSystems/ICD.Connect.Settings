namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlConnectionSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlConnectionSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
