using ICD.Connect.Settings.Originators;
using System.Collections.Generic;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Groups
{
	public interface IGroup : IOriginator
	{
		/// <summary>
		/// Gets the number of items in the group.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		IEnumerable<IOriginator> GetItems();

		/// <summary>
		/// Returns true if the group contains the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool Contains([NotNull] IOriginator item);

		/// <summary>
		/// Adds an item to the group
		/// </summary>
		/// <param name="item"></param>
		/// <returns>true if item was added, false if item was already in the group</returns>
		bool AddItem([NotNull] IOriginator item);

		/// <summary>
		/// Adds items to the group if they aren't already in the group
		/// </summary>
		/// <param name="items"></param>
		void AddItems([NotNull] IEnumerable<IOriginator> items);
	}

	public interface IGroup<TOriginator> : IGroup
		where TOriginator : IOriginator
	{
		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		new IEnumerable<TOriginator> GetItems();

		/// <summary>
		/// Returns true if the group contains the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool Contains([NotNull] TOriginator item);

		/// <summary>
		/// Adds an item to the group
		/// </summary>
		/// <param name="item"></param>
		/// <returns>true if item was added, false if item was already in the group</returns>
		bool AddItem([NotNull] TOriginator item);

		/// <summary>
		/// Adds items to the group if they aren't already in the group
		/// </summary>
		/// <param name="items"></param>
		void AddItems([NotNull] IEnumerable<TOriginator> items);
	}
}
