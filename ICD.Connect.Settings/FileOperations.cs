using System;
using System.Text;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Core;
using ICD.Connect.Settings.Header;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronIO;
using Activator = Crestron.SimplSharp.Reflection.Activator;
#else
using System.IO;
#endif

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Methods for handling serialization and deserialization of settings.
	/// </summary>
	public static class FileOperations
	{
		private const string ROOT_ELEMENT = "IcdConfig";

		private const string CONFIG_LOCAL_PATH = "RoomConfig-Base.xml";
		private const string LICENSE_LOCAL_PATH = "license";

		#region Properties

		public static string IcdConfigPath { get { return PathUtils.GetProgramConfigPath(CONFIG_LOCAL_PATH); } }
		public static string LicensePath { get { return PathUtils.GetProgramConfigPath(LICENSE_LOCAL_PATH); } }

		public static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		#endregion

		#region Methods

		/// <summary>
		/// Applies the settings to the core.
		/// </summary>
		public static void ApplyCoreSettings<TCore, TSettings>(TCore core, TSettings settings)
			where TSettings : ICoreSettings
			where TCore : ICore
		{
			if (core == null)
				throw new ArgumentNullException("core");

			if (settings == null)
				throw new ArgumentNullException("settings");

			Logger.AddEntry(eSeverity.Notice, "Applying settings");

			IDeviceFactory factory = new CoreDeviceFactory(settings);
			core.ApplySettings(settings, factory);

			Logger.AddEntry(eSeverity.Notice, "Finished applying settings");
		}

		/// <summary>
		/// Serializes the settings to disk.
		/// </summary>
		/// <param name="settings"></param>
		public static void SaveSettings(ICoreSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			SaveSettings(settings, true);
		}

		/// <summary>
		/// Serializes the settings to disk.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="backup"></param>
		public static void SaveSettings(ICoreSettings settings, bool backup)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

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
					writer.WriteStartDocument();
					{
						WriteSettingsWarning(writer);
						settings.ToXml(writer, ROOT_ELEMENT);
					}
					writer.WriteEndDocument();
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

			string date = IcdEnvironment.GetLocalTime()
			                            .ToUniversalTime()
			                            .ToString("s")
			                            .Replace(':', '-') + 'Z';

			string newName = string.Format("{0}_Backup_{1}", name, date);
			string newPath = PathUtils.ChangeFilenameWithoutExt(IcdConfigPath, newName);

			Logger.AddEntry(eSeverity.Notice, "Creating settings backup of {0} at {1}", IcdConfigPath, newPath);

			IcdFile.Copy(IcdConfigPath, newPath);

			Logger.AddEntry(eSeverity.Notice, "Finished settings backup");
		}

		/// <summary>
		/// Loads the settings from disk to the core.
		/// </summary>
		public static void LoadCoreSettings<TCore, TSettings>(TCore core)
			where TSettings : ICoreSettings, new()
			where TCore : ICore
		{
			if (core == null)
				throw new ArgumentNullException("core");

			TSettings settings = Activator.CreateInstance<TSettings>();

			// Ensure the new core settings don't default to an id of 0.
			settings.Id = IdUtils.ID_CORE;

			// Load XML config into string
			string configXml = null;
			if (IcdFile.Exists(IcdConfigPath))
			{
				Logger.AddEntry(eSeverity.Notice, "Reading settings from {0}", IcdConfigPath);

				configXml = IcdFile.ReadToEnd(IcdConfigPath, new UTF8Encoding(false));
				configXml = EncodingUtils.StripUtf8Bom(configXml);

				Logger.AddEntry(eSeverity.Notice, "Finished reading settings");
			}
			else
			{
				Logger.AddEntry(eSeverity.Warning, "Failed to find settings at {0}", IcdConfigPath);
			}

			bool save;

			// Save a stub xml file if one doesn't already exist
			if (string.IsNullOrEmpty(configXml))
				save = true;
			else
				ParseXml(settings, configXml, out save);

			ApplyCoreSettings(core, settings);

			if (save)
				SaveSettings(core.CopySettings(), true);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Performs some additional validation/migration before applying XML to the given settings instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="configXml"></param>
		/// <param name="save"></param>
		private static void ParseXml(ICoreSettings settings, string configXml, out bool save)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			save = false;

			ConfigurationHeader header = settings.GetHeader(configXml);

			if (header.ConfigVersion.Equals(new Version(0, 0)))
			{
				Logger.AddEntry(eSeverity.Warning, "Unable to determine configuration version, assuming latest");
			}
			else if (header.ConfigVersion < ConfigurationHeader.CurrentConfigVersion)
			{
				Logger.AddEntry(eSeverity.Warning, "Configuration was generated for an older version (Config={0}, Current={1})",
				                header.ConfigVersion, ConfigurationHeader.CurrentConfigVersion);

				// TODO - Migrate
			}

			settings.ParseXml(configXml);
		}

		/// <summary>
		/// Writes a comment to the xml warning integrators about this XML being overwritten
		/// </summary>
		/// <param name="writer"></param>
		private static void WriteSettingsWarning(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteComment("\nThis configuration is generated automatically.\n" +
			                    "Only change this file if you know what you are doing.\n" +
			                    "Any invalid data, whitespace, and comments will be deleted the next time this is generated.\n");
		}

		#endregion
	}
}
