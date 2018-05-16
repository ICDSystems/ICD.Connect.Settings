using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Settings.SPlusShims
{
	public sealed class SPlusShimManager : IConsoleNode
	{
		private readonly List<ISPlusShim> m_Shims;

		private readonly SafeCriticalSection m_ShimSafeCriticalSection;

		internal SPlusShimManager()
		{
			m_Shims = new List<ISPlusShim>();
			m_ShimSafeCriticalSection = new SafeCriticalSection();
		}

		public void RegisterShim(ISPlusShim shim)
		{
			m_ShimSafeCriticalSection.Enter();
			try
			{
				if (m_Shims.Contains(shim))
					return;

				m_Shims.AddSorted(shim, s => s.Location);
			}
			finally
			{
				m_ShimSafeCriticalSection.Leave();
			}
		}

		public void UnregisterShim(ISPlusShim shim)
		{
			m_ShimSafeCriticalSection.Enter();
			try
			{
				if (!m_Shims.Contains(shim))
					return;

				m_Shims.Remove(shim);
			}
			finally
			{
				m_ShimSafeCriticalSection.Leave();
			}
		}

		#region Console
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public string ConsoleName { get { return "SimplPlusShims"; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public string ConsoleHelp { get { return "The SimplPlus Shims which connect to originators"; } }

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			yield break;
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			addRow("Shim Count", m_Shims.Count);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			yield return new ConsoleCommand("Print", "Prints the list of originator shim(s)", () => PrintShims());
			yield return
				new GenericConsoleCommand<int>("GetInfo", "Gets info about the specified shim", index => PrintShim(index));
		}

		private void PrintShims()
		{
			TableBuilder builder = new TableBuilder("Index", "Simpl Location", "Originator Type", "Originator Name", "Originator Id");

			m_ShimSafeCriticalSection.Enter();

			try
			{
				for (int index = 0; index < m_Shims.Count; index++)
				{
					var shim = m_Shims[index];

					ISPlusOriginatorShim originatorShim = shim as ISPlusOriginatorShim;
					if (originatorShim != null)
					{
						builder.AddRow(index,
						               originatorShim.Location,
						               originatorShim.Originator != null ? originatorShim.Originator.GetType().ToString() : "",
						               originatorShim.Originator != null ? originatorShim.Originator.Name : "",
						               originatorShim.Originator != null ? originatorShim.Originator.Id.ToString() : "");
					}
					else
					{
						builder.AddRow(index, shim.Location, "", "", "");
					}
					
				}
			}
			finally
			{
				m_ShimSafeCriticalSection.Leave();
			}

			IcdConsole.ConsoleCommandResponseLine(builder.ToString());
		}

		private void PrintShim(int index)
		{
			m_ShimSafeCriticalSection.Enter();

			try
			{
				IcdConsole.ConsoleCommandResponseLine(m_Shims.Count > index
														  ? "Location: " + m_Shims[index].Location
														  : "Invalid Index");
			}
			finally
			{
				m_ShimSafeCriticalSection.Leave();
			}
		}

		#endregion
	}
}