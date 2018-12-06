using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.EventArguments;

namespace ICD.Connect.Settings
{
	public interface IOriginatorCollection<TChild> : IEnumerable<TChild>
		where TChild : class, IOriginator
	{
		/// <summary>
		/// Raised when children are added/removed to/from the collection.
		/// </summary>
		event EventHandler OnChildrenChanged;

		event EventHandler<GenericEventArgs<IOriginator>> OnOriginatorAdded;
		event EventHandler<GenericEventArgs<IOriginator>> OnOriginatorRemoved; 

		/// <summary>
		/// Gets the number of children in this collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		TChild this[int id] { get; }

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
		/// Adds the given children to the collection.
		/// </summary>
		/// <param name="children"></param>
		void AddChildren(IEnumerable<TChild> children);

		/// <summary>
		/// Adds the child to the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if a child with the given id already exists.</returns>
		bool AddChild(TChild child);

		/// <summary>
		/// Removes the given children from the collection.
		/// </summary>
		/// <param name="children"></param>
		void RemoveChildren(IEnumerable<TChild> children);

		/// <summary>
		/// Removes the child from the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if the core does not contain the child.</returns>
		bool RemoveChild(TChild child);

		/// <summary>
		/// Returns true if there is a child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool ContainsChild(int id);

		/// <summary>
		/// Returns true if there is a child with the given id of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		bool ContainsChild<TInstanceType>(int id) where TInstanceType : class, TChild;

		/// <summary>
		/// Returns true if there is a child with the given id.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		bool ContainsChild(TChild child);

		/// <summary>
		/// Returns true if at least 1 originator exists of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		bool ContainsChildAny<TInstanceType>(IEnumerable<int> ids) where TInstanceType : class, TChild;

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		TChild GetChild(int id);

		/// <summary>
		/// Gets the first child of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <returns></returns>
		[NotNull]
		TInstanceType GetChild<TInstanceType>() where TInstanceType : class, TChild;

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
		TInstanceType GetChild<TInstanceType>(int id) where TInstanceType : class, TChild;

		/// <summary>
		/// Returns the first instance of the given type from the given instance ids.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		[CanBeNull]
		TInstanceType GetChild<TInstanceType>(IEnumerable<int> ids) where TInstanceType : class, TChild;

		/// <summary>
		/// Returns the first instance of the given type from the given instance ids.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		[CanBeNull]
		TInstanceType GetChild<TInstanceType>(IEnumerable<int> ids, Func<TInstanceType, bool> selector)
			where TInstanceType : class, TChild;

		/// <summary>
		/// Gets all of the children.
		/// </summary>
		/// <returns></returns>
		IEnumerable<TChild> GetChildren();

		/// <summary>
		/// Gets the children with the given ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		IEnumerable<TChild> GetChildren(IEnumerable<int> ids);

		/// <summary>
		/// Gets all of the children of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <returns></returns>
		IEnumerable<TInstanceType> GetChildren<TInstanceType>() where TInstanceType : class, TChild;

		/// <summary>
		/// Gets the children matching the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		IEnumerable<TInstanceType> GetChildren<TInstanceType>(Func<TInstanceType, bool> selector) where TInstanceType : class, TChild;

		/// <summary>
		/// Gets the children with the given ids, matching the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		IEnumerable<TInstanceType> GetChildren<TInstanceType>(IEnumerable<int> ids) where TInstanceType : class, TChild;

		/// <summary>
		/// Gets the children with the given ids, matching the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		IEnumerable<TInstanceType> GetChildren<TInstanceType>(IEnumerable<int> ids, Func<TInstanceType, bool> selector) where TInstanceType : class, TChild;

		IEnumerable<int> GetChildrenIds();

		/// <summary>
		/// Outputs the child with the given id.
		/// Returns false if there is no child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		bool TryGetChild(int id, out TChild child);
	}
}
