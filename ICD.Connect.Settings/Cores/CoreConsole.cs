using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Settings.Cores
{
	public static class CoreConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(ICore instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="addRow"></param>
		public static void BuildConsoleStatus(ICore instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			if (addRow == null)
				throw new ArgumentNullException("addRow");

			string orgString =
				instance.Organization.Id == 0
					? "Unknown"
					: string.Format("{0} - {1}", instance.Organization.Id, instance.Organization.Name);

			addRow("Organization", orgString);
			addRow("Culture", instance.Localization.CurrentCulture.Name);
			addRow("Culture (UI)", instance.Localization.CurrentUiCulture.Name);
			addRow("Core Start Time", instance.CoreStartTime);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(ICore instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}
	}
}
