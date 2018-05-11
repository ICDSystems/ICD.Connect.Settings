using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.SPlusShims.GlobalEvents;

namespace ICD.Connect.Settings.SPlusShims
{
	public abstract class AbstractSPlusShim : ISPlusShim
	{
		#region Private Members

		private static ILoggerService Logger { get { return ServiceProvider.GetService<ILoggerService>(); } }
		private readonly Action<EnvironmentLoadedEventInfo> m_EnvironmentLoadedAction;
		private readonly Action<EnvironmentUnloadedEventInfo> m_EnvironmentUnloadedAction;
		
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
			m_EnvironmentLoadedAction = EnvironmentLoaded;
			m_EnvironmentUnloadedAction = EnvironmentUnloadedAction;
			SPlusGlobalEvents.RegisterCallback(m_EnvironmentLoadedAction);
			SPlusGlobalEvents.RegisterCallback(m_EnvironmentLoadedAction);
			SPlusShimCore.ShimManager.RegisterShim(this);
		}

		protected virtual void EnvironmentLoaded(EnvironmentLoadedEventInfo environmentLoadedEventInfo)
		{
			
		}

		protected virtual void EnvironmentUnloadedAction(EnvironmentUnloadedEventInfo environmentUnloadedEventInfo)
		{
			
		}

		public virtual void Dispose()
		{
			SPlusGlobalEvents.UnregisterCallback(m_EnvironmentLoadedAction);
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