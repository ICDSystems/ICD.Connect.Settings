using System;

namespace ICD.Connect.Settings.Migration
{
	public abstract class AbstractConfigVersionMigrator : IConfigVersionMigrator
	{
		/// <summary>
		/// Gets the starting version for the input configuration.
		/// </summary>
		public abstract Version From { get; }

		/// <summary>
		/// Gets the resulting version for the output configuration.
		/// </summary>
		public abstract Version To { get; }

		/// <summary>
		/// Migrates the input xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public abstract string Migrate(string xml);
	}
}