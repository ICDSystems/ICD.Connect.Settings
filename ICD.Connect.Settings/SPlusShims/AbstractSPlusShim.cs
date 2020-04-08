using System;
using System.Collections.Generic;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.SPlusShims.GlobalEvents;

namespace ICD.Connect.Settings.SPlusShims
{
	public abstract class AbstractSPlusShim : ISPlusShim
	{
		private readonly ServiceLoggingContext m_Logger;

		#region Public Properties

		protected ILoggingContext Logger { get { return m_Logger; } }

		/// <summary>
		/// Location in the SimplWindows program of the S+ Module
		/// Used to aid debugging
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Programmer specified name of the module
		/// Used to aid debugging
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// This callback is raised when the shim wants the S+ class to re-send incoming data to the shim
		/// This is for syncronizing, for example, when an originator is attached.
		/// </summary>
		public event EventHandler OnResyncRequested;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractSPlusShim()
		{
			m_Logger = new ServiceLoggingContext(this);

			Name = "SPlusShim";
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

		protected virtual void RequestResync()
		{
			var handler = OnResyncRequested;
			if (handler != null)
				handler.Raise(this);
		}

		public override string ToString()
		{
			ReprBuilder builder = new ReprBuilder(this);

			if (!String.IsNullOrEmpty(Location))
				builder.AppendProperty("Location", Location);

			return builder.ToString();
		}

		protected static string SPlusSafeString(string input)
		{
			return input ?? String.Empty;
		}

		#endregion

		#region Console
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return String.Format("{0}:{1}", Name, Location); } }

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
			addRow("Name", Name);
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