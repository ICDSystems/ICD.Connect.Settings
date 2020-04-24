using System;
using System.Collections.Generic;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Settings.Originators
{
	public static class OriginatorConsole
	{
		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleNodeBase> GetConsoleNodes(IOriginator instance)
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
		public static void BuildConsoleStatus(IOriginator instance, AddStatusRowDelegate addRow)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			addRow("Category", instance.Category);
			addRow("ID", instance.Id);
			addRow("UUID", instance.Uuid);
			addRow("Name", instance.Name);
			addRow("CombineName", instance.Name);
			addRow("Description", instance.Description);
			addRow("Hide", instance.Hide);
			addRow("Disable", instance.Disable);
			addRow("Order", instance.Order);
			addRow("Serialize", instance.Serialize);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IEnumerable<IConsoleCommand> GetConsoleCommands(IOriginator instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");

			yield break;
		}
	}
}
