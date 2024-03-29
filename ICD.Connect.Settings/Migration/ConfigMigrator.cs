﻿using System;
using System.Collections.Generic;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Migration.Migrators;

namespace ICD.Connect.Settings.Migration
{
	/// <summary>
	/// Features for processing an old core configuration into a newer format.
	/// </summary>
	public static class ConfigMigrator
	{
		private static readonly Dictionary<Version, IConfigVersionMigrator> s_Migrators;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static ConfigMigrator()
		{
			s_Migrators = new Dictionary<Version, IConfigVersionMigrator>();

			RegisterMigrator(new ConfigVersionMigrator_2x0_To_3x0());
			RegisterMigrator(new ConfigVersionMigrator_3x0_To_3x1());
			RegisterMigrator(new ConfigVersionMigrator_3x1_To_4x0());
			RegisterMigrator(new ConfigVersionMigrator_4x0_To_5x0());
		}

		/// <summary>
		/// Migrates the given core configuration and returns a new core configuration.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="version"></param>
		/// <param name="resulting"></param>
		/// <returns></returns>
		public static string Migrate(string xml, Version version, out Version resulting)
		{
			IConfigVersionMigrator migrator;
			while (s_Migrators.TryGetValue(version, out migrator))
			{
				xml = migrator.Migrate(xml);
				version = migrator.To;
			}

			xml = XmlUtils.Format(xml);

			resulting = version.Clone() as Version;
			return xml;
		}

		/// <summary>
		/// Adds the the given migrator to the table of version migrators.
		/// </summary>
		/// <param name="migrator"></param>
		public static void RegisterMigrator(IConfigVersionMigrator migrator)
		{
			if (migrator == null)
				throw new ArgumentNullException("migrator");

			s_Migrators.Add(migrator.From, migrator);
		}
	}
}
