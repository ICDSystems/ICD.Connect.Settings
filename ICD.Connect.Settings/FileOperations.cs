using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Comparers;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.Header;
using ICD.Connect.Settings.Migration;
using ICD.Connect.Settings.Utils;
using ICD.Connect.Settings.Validation;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Methods for handling serialization and deserialization of settings.
	/// </summary>
	public static class FileOperations
	{
		private const string ROOT_ELEMENT = "SystemConfig";

		private const string CONFIG_LOCAL_PATH = "SystemConfig.xml";
		private const string CONFIG_LOCAL_PATH_OLD = "RoomConfig-Base.xml";
		private const string LICENSE_LOCAL_PATH = "license";
		private const string SYSTEM_KEY_LOCAL_PATH = "systemkey";

		#region Properties

		/// <summary>
		/// Gets the path to the configuration file.
		/// </summary>
		public static string SystemConfigPath { get { return PathUtils.GetProgramConfigPath(CONFIG_LOCAL_PATH); } }

		/// <summary>
		/// Backwards compatibility.
		/// </summary>
		[Obsolete("Use SystemConfigPath instead")]
		private static string RoomConfigPath { get { return PathUtils.GetProgramConfigPath(CONFIG_LOCAL_PATH_OLD); } }

		[Obsolete("Use SystemKeyPath instead")]
		public static string LicensePath { get { return PathUtils.GetProgramConfigPath(LICENSE_LOCAL_PATH); } }

		public static string SystemKeyPath { get { return PathUtils.GetProgramConfigPath(SYSTEM_KEY_LOCAL_PATH); } }

		private static ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		#endregion

		#region Methods

		/// <summary>
		/// Applies the settings to the core.
		/// </summary>
		public static void ApplyCoreSettings<TCore, TSettings>(TCore core, TSettings settings)
			where TSettings : class, ICoreSettings
			where TCore : class, ICore
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

			string path = SystemConfigPath;
			string directory = IcdPath.GetDirectoryName(path);
			IcdDirectory.CreateDirectory(directory);

			Logger.AddEntry(eSeverity.Notice, "Saving settings to {0}", path);

			using (IcdFileStream stream = IcdFile.Open(path, eIcdFileMode.Create))
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
		/// Loads the settings from disk to the core.
		/// </summary>
		public static void LoadCoreSettings<TCore, TSettings>(TCore core)
			where TSettings : class, ICoreSettings, new()
			where TCore : class, ICore
		{
			if (core == null)
				throw new ArgumentNullException("core");

			TSettings settings = ReflectionUtils.CreateInstance<TSettings>();

			// Ensure the new core settings don't default to an id of 0.
			settings.Id = IdUtils.ID_CORE;

			// Backwards compatibility
			if (!IcdFile.Exists(SystemConfigPath) &&
			    IcdFile.Exists(RoomConfigPath))
			{
				BackupSettings();
				Logger.AddEntry(eSeverity.Notice, "Renaming {0} to {1}", CONFIG_LOCAL_PATH_OLD, CONFIG_LOCAL_PATH);
				IcdFile.Move(RoomConfigPath, SystemConfigPath);
			}

			// Load XML config into string
			string configXml = null;
			if (IcdFile.Exists(SystemConfigPath))
			{
				Logger.AddEntry(eSeverity.Notice, "Reading settings from {0}", SystemConfigPath);

				configXml = IcdFile.ReadToEnd(SystemConfigPath, new UTF8Encoding(false));
				configXml = EncodingUtils.StripUtf8Bom(configXml);

				try
				{
					if (!string.IsNullOrEmpty(configXml))
						new IcdXmlDocument().LoadXml(configXml);
				}
				catch (IcdXmlException e)
				{
					Logger.AddEntry(eSeverity.Error, "Failed to load settings - Error reading XML at line {0} position {1}",
					                e.LineNumber, e.LinePosition);
					return;
				}

				Logger.AddEntry(eSeverity.Notice, "Finished reading settings");
			}
			else
			{
				Logger.AddEntry(eSeverity.Warning, "Failed to find settings at {0}", SystemConfigPath);
			}

			bool save;

			// Save a stub xml file if one doesn't already exist
			if (string.IsNullOrEmpty(configXml))
				save = true;
			else
				ParseXml(settings, configXml, out save);

			SettingsValidationResult critical;
			if (!ValidateSettings(settings, out critical))
			{
				Logger.AddEntry(eSeverity.Critical, "Aborting load of settings - {0} failed to validate - {1}", critical.Source,
				                critical.Message);
				return;
			}

			if (save)
				SaveSettings(settings, true);
			else
				BackupSettings();

			ApplyCoreSettings(core, settings);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Copies the existing settings to a new path with the current date.
		/// </summary>
		private static void BackupSettings()
		{
			// Backwards compatibility
			string configPath =
				IcdFile.Exists(SystemConfigPath)
					? SystemConfigPath
					: IcdFile.Exists(RoomConfigPath)
						? RoomConfigPath
						: SystemConfigPath;

			if (!IcdFile.Exists(configPath))
				return;

			string name = IcdPath.GetFileNameWithoutExtension(configPath);

			string date = IcdEnvironment.GetUtcTime()
			                            .ToString("s")
			                            .Replace(':', '-') + 'Z';

			string newName = string.Format("{0}_Backup_{1}.xml", name, date);
			string newPath = PathUtils.GetProgramDataPath(newName);

			Logger.AddEntry(eSeverity.Notice, "Creating settings backup of {0} at {1}", configPath, newPath);

			IcdDirectory.CreateDirectory(PathUtils.ProgramDataPath);
			IcdFile.Copy(configPath, newPath);

			Logger.AddEntry(eSeverity.Notice, "Finished settings backup");
		}

		/// <summary>
		/// Returns false if there is a critical error (or worse) during settings validation.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="critical"></param>
		/// <returns></returns>
		private static bool ValidateSettings([NotNull] ICoreSettings settings, out SettingsValidationResult critical)
		{
			Logger.AddEntry(eSeverity.Notice, "Validating settings");

			critical = null;
			bool output = true;

			Dictionary<eSeverity, int> severityCount = new Dictionary<eSeverity, int>();

			foreach (SettingsValidationResult result in settings.Validate())
			{
				Logger.AddEntry(result.Severity, "{0} - {1}", result.Source, result.Message);

				severityCount[result.Severity] = severityCount.GetDefault(result.Severity) + 1;
				if (result.Severity > eSeverity.Critical)
					continue;

				critical = critical ?? result;
				output = false;
			}

			// Lower is worse
			eSeverity min = severityCount.Count > 0 ? (eSeverity)severityCount.Keys.Cast<int>().Min() : eSeverity.Notice;
			min = min < eSeverity.Notice ? min : eSeverity.Notice;
			Logger.AddEntry(min,
			                "Finished validating settings ({0} emergency, {1} alert, {2} critical, {3} error, {4} warning)",
			                severityCount.GetDefault(eSeverity.Emergency),
			                severityCount.GetDefault(eSeverity.Alert),
			                severityCount.GetDefault(eSeverity.Critical),
			                severityCount.GetDefault(eSeverity.Error),
			                severityCount.GetDefault(eSeverity.Warning));

			return output;
		}

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

			if (UndefinedVersionEqualityComparer.Instance.Equals(header.ConfigVersion, new Version(0, 0)))
			{
				Logger.AddEntry(eSeverity.Warning, "Unable to determine configuration version, assuming latest");
			}
			else if (UndefinedVersionComparer.Instance.Compare(header.ConfigVersion, ConfigurationHeader.CurrentConfigVersion) < 0)
			{
				Logger.AddEntry(eSeverity.Warning, "Configuration was generated for an older version (Config={0}, Current={1})",
				                header.ConfigVersion, ConfigurationHeader.CurrentConfigVersion);

				try
				{
					Version resulting;
					configXml = ConfigMigrator.Migrate(configXml, header.ConfigVersion, out resulting);
					save = true;

					Logger.AddEntry(eSeverity.Notice, "Migrated config from {0} to {1}", header.ConfigVersion, resulting);
				}
				catch (Exception e)
				{
					Logger.AddEntry(eSeverity.Error, e, "Failed to migrate configuration - {0}", e.Message);
				}
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
