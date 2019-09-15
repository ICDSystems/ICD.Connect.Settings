using ICD.Connect.Settings.Originators;
using System.Collections.Generic;

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
	}

	public interface IGroup<TOriginator> : IGroup
		where TOriginator : IOriginator
	{
		/// <summary>
		/// Gets the items in the group.
		/// </summary>
		new IEnumerable<TOriginator> GetItems();
	}
}
