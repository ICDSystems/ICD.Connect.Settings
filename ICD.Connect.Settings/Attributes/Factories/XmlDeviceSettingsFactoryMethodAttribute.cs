namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlDeviceSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlDeviceSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
