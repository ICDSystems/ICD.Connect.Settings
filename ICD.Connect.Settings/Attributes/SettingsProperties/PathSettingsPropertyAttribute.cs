namespace ICD.Connect.Settings.Attributes.SettingsProperties
{
	public sealed class PathSettingsPropertyAttribute : AbstractSettingsPropertyAttribute
	{
		private readonly string m_BasePath;
		private readonly string m_Extension;

		/// <summary>
		/// Gets the base path for the path settings.
		/// </summary>
		public string BasePath { get { return m_BasePath; } }

		/// <summary>
		/// Gets the extension for the path settings.
		/// </summary>
		public string Extension { get { return m_Extension; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="extension"></param>
		public PathSettingsPropertyAttribute(string extension)
			: this(null, extension)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="basePath"></param>
		/// <param name="extension"></param>
		public PathSettingsPropertyAttribute(string basePath, string extension)
		{
			m_BasePath = basePath;
			m_Extension = extension;
		}
	}
}
