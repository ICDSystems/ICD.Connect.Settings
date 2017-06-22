namespace ICD.Connect.Settings.Attributes.Factories
{
	public sealed class XmlRoomSettingsFactoryMethodAttribute : AbstractXmlFactoryMethodAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="typeName"></param>
		public XmlRoomSettingsFactoryMethodAttribute(string typeName)
			: base(typeName)
		{
		}
	}
}
