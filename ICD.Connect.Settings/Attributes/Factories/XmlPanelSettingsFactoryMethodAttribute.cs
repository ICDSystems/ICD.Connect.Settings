namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlPanelSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlPanelSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
