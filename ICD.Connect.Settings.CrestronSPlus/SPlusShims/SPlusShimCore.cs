using ICD.Common.Logging;
using ICD.Common.Logging.Loggers;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API;

namespace ICD.Connect.Settings.CrestronSPlus.SPlusShims
{
	public static class SPlusShimCore
	{
		public static SPlusShimManager ShimManager { get; private set; }

		static SPlusShimCore()
		{
			// Set up logging.
			LoggingCore logger = new LoggingCore();
			logger.AddLogger(new IcdErrorLogger());
			logger.SeverityLevel = eSeverity.Warning;

			ServiceProvider.TryAddService<ILoggerService>(logger);

			// Create Wrapper instance
			ShimManager = new SPlusShimManager();
			ApiConsole.RegisterChild(ShimManager);
		}
	}
}