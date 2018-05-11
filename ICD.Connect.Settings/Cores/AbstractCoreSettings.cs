namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCoreSettings : AbstractSettings, ICoreSettings
	{
		private const string ROOT_ELEMENT = "IcdConfig";

		public abstract SettingsCollection OriginatorSettings { get; }

		/// <summary>
		/// Gets the xml element.
		/// </summary>
		protected override string Element { get { return ROOT_ELEMENT; } }
	}
}
