namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlDestinationGroupSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlDestinationGroupSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
