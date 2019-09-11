using System;
using System.Globalization;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Globalization;

namespace ICD.Connect.Settings.Localization
{
	public sealed class Localization
	{
		public enum e24HourOverride
		{
			/// <summary>
			/// No conversion is applied to the culture.
			/// </summary>
			None,

			/// <summary>
			/// The culture is converted to 12 hour time format.
			/// </summary>
			Override12Hour,

			/// <summary>
			/// The culture is converted to 24 hour time format.
			/// </summary>
			Override24Hour
		}

		/// <summary>
		/// Raised when a culture changes.
		/// </summary>
		public event EventHandler OnCultureChanged;

		private CultureInfo m_CurrentCulture;
		private CultureInfo m_CurrentUiCulture;
		private e24HourOverride m_24HourOverride;

		#region Properties

		/// <summary>
		/// Gets the current culture (data formatting).
		/// </summary>
		[NotNull]
		public CultureInfo CurrentCulture
		{
			get { return m_CurrentCulture; }
			private set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				m_CurrentCulture = value;

				OnCultureChanged.Raise(this);
			}
		}

		/// <summary>
		/// Gets the current UI culture (localization strings).
		/// </summary>
		[NotNull]
		public CultureInfo CurrentUiCulture
		{
			get { return m_CurrentUiCulture; }
			private set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				m_CurrentUiCulture = value;

				OnCultureChanged.Raise(this);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public Localization()
		{
			m_CurrentCulture = IcdCultureInfo.CurrentCulture;
			m_CurrentUiCulture = IcdCultureInfo.CurrentUICulture;
		}

		#region Methods

		/// <summary>
		/// Sets the current culture by name.
		/// </summary>
		/// <param name="name"></param>
		public void SetCulture(string name)
		{
			CurrentCulture = CreateCulture(name);
		}

		/// <summary>
		/// Sets the UI culture by name.
		/// </summary>
		/// <param name="name"></param>
		public void SetUiCulture(string name)
		{
			CurrentUiCulture = CreateCulture(name);
		}

		/// <summary>
		/// Sets the 24 hour override mode.
		/// </summary>
		/// <param name="mode"></param>
		public void Set24HourOverride(e24HourOverride mode)
		{
			if (mode == m_24HourOverride)
				return;

			m_24HourOverride = mode;

			CurrentCulture = CreateCulture(m_CurrentCulture.Name);
			CurrentUiCulture = CreateCulture(m_CurrentUiCulture.Name);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates a new CultureInfo instance given the provided culture name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private CultureInfo CreateCulture(string name)
		{
			IcdCultureInfo output = new IcdCultureInfo(name);

			if (output.IsNeutralCulture)
				throw new ArgumentException(
					"A neutral culture does not provide enough information to display the correct numeric format");

			return Apply24HourOverride(output);
		}

		/// <summary>
		/// Applies the configured 24 hour override to the given culture.
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		private CultureInfo Apply24HourOverride(CultureInfo culture)
		{
			if (culture == null)
				throw new ArgumentNullException("culture");

			switch (m_24HourOverride)
			{
				case e24HourOverride.None:
					break;

				case e24HourOverride.Override12Hour:
					culture.ConvertTo12HourCulture();
					break;

				case e24HourOverride.Override24Hour:
					culture.ConvertTo24HourCulture();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return culture;
		}

		#endregion

		#region Settings

		/// <summary>
		/// Reverts cultures to their defaults.
		/// </summary>
		public void ClearSettings()
		{
			m_24HourOverride = e24HourOverride.None;
			CurrentCulture = IcdCultureInfo.CurrentCulture;
			CurrentUiCulture = IcdCultureInfo.CurrentUICulture;
		}

		/// <summary>
		/// Copies the current state onto the given settings instance.
		/// </summary>
		/// <param name="settings"></param>
		public void CopySettings(LocalizationSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			settings.Override24Hour = m_24HourOverride;
			settings.Culture = CurrentCulture.Name;
			settings.UiCulture = CurrentUiCulture.Name;
		}

		/// <summary>
		/// Applies the given settings to the current state.
		/// </summary>
		/// <param name="settings"></param>
		public void ApplySettings(LocalizationSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			ClearSettings();

			Set24HourOverride(settings.Override24Hour);

			if (!string.IsNullOrEmpty(settings.Culture))
				SetCulture(settings.Culture);

			if (!string.IsNullOrEmpty(settings.UiCulture))
				SetUiCulture(settings.UiCulture);
		}

		#endregion
	}
}