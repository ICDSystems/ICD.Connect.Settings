using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;

namespace ICD.Connect.Settings.SPlusShims
{
	public abstract class AbstractSPlusShim : ISPlusShim
	{
		#region Private Members

		private static ILoggerService Logger { get { return ServiceProvider.GetService<ILoggerService>(); } }
		
		#endregion

		#region Public Properties

		/// <summary>
		/// The Simpl Windows Location, set by S+
		/// </summary>
		[PublicAPI("S+")]
		public string Location { get; set; }

		#endregion

		[PublicAPI("S+")]
		public AbstractSPlusShim()
		{
			SPlusShimCore.ShimManager.RegisterShim(this);
		}

		public virtual void Dispose()
		{
            SPlusShimCore.ShimManager.UnregisterShim(this);
		}

		#region Private/Protected Helpers

		protected void Log(eSeverity severity, string message)
		{
			Logger.AddEntry(severity, "{0} - {1}", this, message);
		}

		protected void Log(eSeverity severity, string message, params object[] args)
		{
			message = string.Format(message, args);
			Log(severity, message);
		}

		#endregion
	}
}