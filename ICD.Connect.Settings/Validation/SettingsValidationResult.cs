using ICD.Common.Utils.Services.Logging;

namespace ICD.Connect.Settings.Validation
{
	public sealed class SettingsValidationResult
	{
		public ISettings Source { get; set; }
		public eSeverity Severity { get; set; }
		public string Message { get; set; }
	}
}