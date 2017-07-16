namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlPartitionSettingsFactoryMethod : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlPartitionSettingsFactoryMethod(string typeName)
			: base(typeName)
		{
		}
	}
}
