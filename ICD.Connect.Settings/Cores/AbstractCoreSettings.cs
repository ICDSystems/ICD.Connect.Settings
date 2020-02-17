using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Connect.Settings.Header;
using ICD.Connect.Settings.Validation;

namespace ICD.Connect.Settings.Cores
{
	public abstract class AbstractCoreSettings : AbstractSettings, ICoreSettings
	{
		/// <summary>
		/// Gets the child originator settings collection.
		/// </summary>
		public abstract SettingsCollection OriginatorSettings { get; }

		/// <summary>
		/// Parses and returns only the header portion from the full config.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public abstract ConfigurationHeader GetHeader(string config);

		/// <summary>
		/// Validates the core settings as a whole.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SettingsValidationResult> Validate()
		{
			return Validate(this);
		}

		/// <summary>
		/// Validates this settings instance against the core settings as a whole.
		/// </summary>
		/// <param name="coreSettings"></param>
		/// <returns></returns>
		public override IEnumerable<SettingsValidationResult> Validate([NotNull] ICoreSettings coreSettings)
		{
			if (coreSettings == null)
				throw new ArgumentNullException("coreSettings");

			foreach (SettingsValidationResult result in BaseValidate(coreSettings))
				yield return result;

			foreach (SettingsValidationResult result in OriginatorSettings.SelectMany(s => s.Validate(coreSettings)))
				yield return result;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <param name="coreSettings"></param>
		/// <returns></returns>
		private IEnumerable<SettingsValidationResult> BaseValidate(ICoreSettings coreSettings)
		{
			return base.Validate(coreSettings);
		}
	}
}
