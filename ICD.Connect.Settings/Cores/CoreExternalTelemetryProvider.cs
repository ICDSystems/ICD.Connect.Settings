using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Originators;
using ICD.Connect.Telemetry;

namespace ICD.Connect.Settings.Cores
{
	public sealed class CoreExternalTelemetryProvider : AbstractExternalTelemetryProvider<ICore>,
	                                                    ICoreExternalTelemetryProvider
	{
		private IOriginator m_Theme;

		public string SoftwareVersion { get { return Parent.GetType().GetAssembly().GetName().Version.ToString(); } }

		public string SoftwareInformationalVersion
		{
			get
			{
				string version;
				return Parent.GetType().GetAssembly().TryGetInformationalVersion(out version) ? version : null;
			}
		}

		public string ThemeName
		{
			get
			{
				IOriginator theme = GetTheme();
				return theme == null ? null : theme.Name;
			}
		}

		public string ThemeVersion
		{
			get
			{
				IOriginator theme = GetTheme();
				return theme == null ? null : theme.GetType().GetAssembly().GetName().Version.ToString();
			}
		}

		public string ThemeInformationalVersion
		{
			get
			{
				IOriginator theme = GetTheme();
				string version;
				return theme == null
					       ? null
					       : theme.GetType().GetAssembly().TryGetInformationalVersion(out version)
						         ? version
						         : null;
			}
		}

		[CanBeNull]
		private IOriginator GetTheme()
		{
			// Disgusting hack time - Don't want Settings to have a dependency on Themes
			return m_Theme ??
			       (m_Theme = Parent.Originators
			                        .GetChildren()
			                        .FirstOrDefault(o => o.GetType().GetInterfaces().Any(i => i.Name == "ITheme")));
		}
	}
}
