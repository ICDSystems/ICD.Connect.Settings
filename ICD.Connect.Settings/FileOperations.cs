﻿using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Core;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.Reflection;
#else
using System;
using System.IO;
#endif

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Methods for handling serialization and deserialization of settings.
	/// </summary>
	public static class FileOperations
	{
		private const string CONFIG_LOCAL_PATH = "RoomConfig-Base.xml";

		public static string IcdConfigPath { get { return PathUtils.GetProgramConfigPath(CONFIG_LOCAL_PATH); } }

		public static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		/// <summary>
		/// Applies the settings to the core.
		/// </summary>
		public static void ApplyCoreSettings<TCore, TSettings>(TCore core, TSettings settings)
			where TSettings : ICoreSettings
			where TCore : ICore
		{
			Logger.AddEntry(eSeverity.Notice, "Applying settings");

			IDeviceFactory factory = new CoreDeviceFactory(settings);
			core.ApplySettings(settings, factory);

			Logger.AddEntry(eSeverity.Notice, "Finished applying settings");
		}

		/// <summary>
		/// Serializes the settings to disk.
		/// </summary>
		/// <param name="settings"></param>
		public static void SaveSettings(ISettings settings)
		{
			SaveSettings(settings, false);
		}

		/// <summary>
		/// Serializes the settings to disk.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="backup"></param>
		public static void SaveSettings(ISettings settings, bool backup)
		{
			if (backup)
				BackupSettings();

			string path = IcdConfigPath;
			string directory = IcdPath.GetDirectoryName(path);
			IcdDirectory.CreateDirectory(directory);

			Logger.AddEntry(eSeverity.Notice, "Saving settings to {0}", path);

			using (IcdFileStream stream = IcdFile.Open(path, FileMode.Create))
			{
				using (IcdXmlTextWriter writer = new IcdXmlTextWriter(stream, new UTF8Encoding(false)))
				{
					WriteSettingsWarning(writer);
					settings.ToXml(writer);
				}
			}

			Logger.AddEntry(eSeverity.Notice, "Finished saving settings");
		}

		/// <summary>
		/// Copies the existing settings to a new path with the current date.
		/// </summary>
		public static void BackupSettings()
		{
			if (!IcdFile.Exists(IcdConfigPath))
				return;

			string name = IcdPath.GetFileNameWithoutExtension(IcdConfigPath);
			string date = IcdEnvironment.GetLocalTime().ToString("MM-dd-yyyy_HH-mm");
			string newName = string.Format("{0}_Backup_{1}", name, date);
			string newPath = PathUtils.ChangeFilenameWithoutExt(IcdConfigPath, newName);

			Logger.AddEntry(eSeverity.Notice, "Creating settings backup of {0} at {1}", IcdConfigPath, newPath);

			IcdFile.Copy(IcdConfigPath, newPath);

			Logger.AddEntry(eSeverity.Notice, "Finished settings backup");
		}

		/// <summary>
		/// Writes a comment to the xml warning integrators about this XML being overwritten
		/// </summary>
		/// <param name="writer"></param>
		private static void WriteSettingsWarning(IcdXmlTextWriter writer)
		{
			writer.WriteComment("\nThis configuration is generated automatically.\n" +
			                    "Only change this file if you know what you are doing.\n" +
			                    "Any invalid data, whitespace, and comments will be deleted the next time this is generated.\n");
		}

		/// <summary>
		/// Loads the settings from disk to the core.
		/// </summary>
		public static void LoadCoreSettings<TCore, TSettings>(TCore core)
			where TSettings : ICoreSettings, new()
			where TCore : ICore
		{
			TSettings settings = Activator.CreateInstance<TSettings>();

			// Ensure the new core settings don't default to an id of 0.
			settings.Id = 1;

			LoadCoreSettings(core, settings);
		}

		/// <summary>
		/// Loads the settings from disk to the core.
		/// </summary>
		public static void LoadCoreSettings<TCore, TSettings>(TCore core, TSettings settings)
			where TSettings : ICoreSettings
			where TCore : ICore
		{
			string path = IcdConfigPath;

			Logger.AddEntry(eSeverity.Notice, "Loading settings from {0}", path);

			// Load XML config into string
			string configXml = null;
			if (IcdFile.Exists(path))
				configXml = IcdFile.ReadToEnd(path, Encoding.UTF8);

			bool save = false;

			Logger.AddEntry(eSeverity.Notice, "Finished loading settings");

			// Save a stub xml file if one doesn't already exist
			if (string.IsNullOrEmpty(configXml))
				save = true;
			else
				settings.ParseXml(configXml);

			ApplyCoreSettings(core, settings);

			if (save)
				SaveSettings(core.CopySettings(), true);
		}
	}
}
