using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.SPlusShims.GlobalEvents;

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

		protected AbstractSPlusShim()
		{
			SPlusGlobalEvents.RegisterCallback<EnvironmentLoadedEventInfo>(EnvironmentLoaded);
			SPlusGlobalEvents.RegisterCallback<EnvironmentUnloadedEventInfo>(EnvironmentUnloaded);

			SPlusShimCore.ShimManager.RegisterShim(this);
		}

		protected virtual void EnvironmentLoaded(EnvironmentLoadedEventInfo environmentLoadedEventInfo)
		{
		}

		protected virtual void EnvironmentUnloaded(EnvironmentUnloadedEventInfo environmentUnloadedEventInfo)
		{
		}

		public virtual void Dispose()
		{
			SPlusGlobalEvents.UnregisterCallback<EnvironmentLoadedEventInfo>(EnvironmentLoaded);
			SPlusGlobalEvents.UnregisterCallback<EnvironmentUnloadedEventInfo>(EnvironmentUnloaded);

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

		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);

			if (!string.IsNullOrEmpty(Location))
				builder.AppendProperty("Location", Location);

			return builder.ToString();
		}

		#endregion

		#region Console
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return "SPlusShim:" + Location; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return "Shim for interfacing with S+ Code"; } }

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public virtual void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			addRow("Location", Location);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			yield break;
		}

		#endregion

	}
}