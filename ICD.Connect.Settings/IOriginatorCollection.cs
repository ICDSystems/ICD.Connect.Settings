using System;
using System.Collections.Generic;

namespace ICD.Connect.Settings
{
	public interface IOriginatorCollection<TChild> : IEnumerable<TChild>
		where TChild : IOriginator
	{
		event EventHandler OnChildrenChanged;

		/// <summary>
		/// Gets the number of children in this collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Clears the collection.
		/// </summary>
		void Clear();

		/// <summary>
		/// Clears and sets the children.
		/// </summary>
		/// <returns></returns>
		void SetChildren(IEnumerable<TChild> children);

		/// <summary>
		/// Gets all of the children.
		/// </summary>
		/// <returns></returns>
		IEnumerable<TChild> GetChildren();

		/// <summary>
		/// Gets all of the children of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <returns></returns>
		IEnumerable<TInstanceType> GetChildren<TInstanceType>() where TInstanceType : TChild;

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		TChild GetChild(int id);

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		TChild this[int id] { get; }

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		TInstanceType GetChild<TInstanceType>(int id) where TInstanceType : TChild;

		/// <summary>
		/// Outputs the child with the given id.
		/// Returns false if there is no child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		bool TryGetChild(int id, out TChild child);

		/// <summary>
		/// Returns true if there is a child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool ContainsChild(int id);

		/// <summary>
		/// Adds the child to the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if a child with the given id already exists.</returns>
		bool AddChild(TChild child);

		/// <summary>
		/// Removes the child from the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if the core does not contain the child.</returns>
		bool RemoveChild(TChild child);
	}
}
