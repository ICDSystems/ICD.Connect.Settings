namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlDestinationSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlDestinationSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
