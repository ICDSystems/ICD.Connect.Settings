using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Originators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICD.Connect.Settings.Groups
{
	public abstract class AbstractGroup<TOriginator, TSettings> : AbstractOriginator<TSettings>, IGroup<TOriginator>
		where TSettings : IGroupSettings, new()
		where TOriginator : class, IOriginator
	{
		private readonly List<TOriginator> m_Items;
		private readonly IcdHashSet<TOriginator> m_ItemsSet; 
		private readonly SafeCriticalSection m_ItemsSection;

		#region Properties

		/// <summary>
		/// Gets the number of items in the group.
		/// </summary>
		public int Count { get { return m_ItemsSection.Execute(() => m_Items.Count); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractGroup()
		{
			m_Items = new List<TOriginator>();
			m_ItemsSet = new IcdHashSet<TOriginator>();
			m_ItemsSection = new SafeCriticalSection();
		}

		#region Methods

		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		public IEnumerable<TOriginator> GetItems()
		{
			return m_ItemsSection.Execute(() => m_Items.ToList());
		}

		/// <summary>
		/// Returns true if the group contains the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(TOriginator item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			return m_ItemsSection.Execute(() => m_ItemsSet.Contains(item));
		}

		/// <summary>
		/// Returns true if the group contains the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool IGroup.Contains(IOriginator item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			TOriginator cast = item as TOriginator;
			return cast != null && Contains(cast);
		}

		/// <summary>
		/// Adds an item to the group
		/// </summary>
		/// <param name="item"></param>
		/// <returns>true if item was added, false if item was already in the group</returns>
		public bool AddItem(IOriginator item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			TOriginator itemCast = item as TOriginator;

			if (itemCast == null)
				throw new ArgumentException(string.Format("Item is not of type {0}",typeof(TOriginator)),"item");

			return AddItem(itemCast);
		}

		/// <summary>
		/// Adds items to the group if they aren't already in the group
		/// </summary>
		/// <param name="items"></param>
		public void AddItems(IEnumerable<IOriginator> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			try
			{
				IEnumerable<TOriginator> itemsCast = items.Cast<TOriginator>();
				AddItems(itemsCast);
			}
			catch (InvalidCastException e)
			{
				throw new ArgumentException(string.Format("One or more items not of type {0}", typeof(TOriginator)), "items");
			}
		}

		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		IEnumerable<IOriginator> IGroup.GetItems()
		{
			return GetItems().Cast<IOriginator>();
		}

		/// <summary>
		/// Adds an item to the group
		/// </summary>
		/// <param name="item"></param>
		/// <returns>true if item was added, false if item was already in the group</returns>
		public bool AddItem(TOriginator item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			m_ItemsSection.Enter();

			try
			{
				if (m_ItemsSet.Contains(item))
					return false;

				m_Items.Add(item);
				m_ItemsSet.Add(item);
			}
			finally
			{
				m_ItemsSection.Leave();
			}

			return true;
		}

		/// <summary>
		/// Adds items to the group if they aren't already in the group
		/// </summary>
		/// <param name="items"></param>
		public void AddItems(IEnumerable<TOriginator> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			m_ItemsSection.Enter();

			try
			{
				foreach (TOriginator item in items)
					AddItem(item);
			}
			finally
			{
				m_ItemsSection.Leave();
			}
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_ItemsSection.Execute(() => m_Items.Clear());
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.SetIds(m_ItemsSection.Execute(() => m_Items.Select(i => i.Id).ToList()));
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_ItemsSection.Enter();

			try
			{
				m_Items.Clear();
				m_ItemsSet.Clear();

				IEnumerable<TOriginator> items = GetOriginatorsSkipExceptions(settings.GetIds(), factory);

				m_Items.AddRange(items);
				m_ItemsSet.AddRange(m_Items);
			}
			finally
			{
				m_ItemsSection.Leave();
			}
		}

		private IEnumerable<TOriginator> GetOriginatorsSkipExceptions(IEnumerable<int> ids,
		                                                              IDeviceFactory factory)
		{
			foreach (int id in ids.Distinct())
			{
				TOriginator output;

				try
				{
					output = factory.GetOriginatorById<TOriginator>(id);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Failed to instantiate {0} with id {1} - {2}", typeof(TOriginator).Name, id, e.Message);
					continue;
				}

				yield return output;
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			foreach (IConsoleNodeBase node in GroupConsole.GetConsoleNodes(this))
				yield return node;
		}

		/// <summary>
		/// Wrokaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			GroupConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			foreach (IConsoleCommand command in GroupConsole.GetConsoleCommands(this))
				yield return command;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
