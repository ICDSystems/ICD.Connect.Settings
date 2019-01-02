using System;

namespace ICD.Connect.Settings.Migration
{
	public interface IConfigVersionMigrator
	{
		/// <summary>
		/// Gets the starting version for the input configuration.
		/// </summary>
		Version From { get; }

		/// <summary>
		/// Gets the resulting version for the output configuration.
		/// </summary>
		Version To { get; }

		/// <summary>
		/// Migrates the input xml.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		string Migrate(string xml);
	}
}
