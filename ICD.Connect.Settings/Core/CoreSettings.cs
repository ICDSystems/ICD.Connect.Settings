using System;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Settings.Core
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class CoreSettings : AbstractCoreSettings
	{
		private const string FACTORY_NAME = "Core";

		private readonly SettingsCollection m_OriginatorSettings;

		public override string FactoryName { get { return FACTORY_NAME; } }

		public override Type OriginatorType { get { return typeof(Core); } }

		public override SettingsCollection OriginatorSettings { get { return m_OriginatorSettings; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public CoreSettings()
		{
			m_OriginatorSettings = new SettingsCollection();
		}
	}
}