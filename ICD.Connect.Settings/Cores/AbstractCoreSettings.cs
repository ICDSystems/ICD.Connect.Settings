﻿using ICD.Connect.Settings.Header;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCoreSettings : AbstractSettings, ICoreSettings
	{
		/// <summary>
		/// Gets the child originator settings collection.
		/// </summary>
		public abstract SettingsCollection OriginatorSettings { get; }

		/// <summary>
		/// Parses and returns only the header portion from the full XML config.
		/// </summary>
		/// <param name="configXml"></param>
		/// <returns></returns>
		public abstract ConfigurationHeader GetHeader(string configXml);
	}
}
