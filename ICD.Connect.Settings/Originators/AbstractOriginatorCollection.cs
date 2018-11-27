using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Comparers;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.Originators
{
	public abstract class AbstractOriginatorCollection<TChild> : IOriginatorCollection<TChild>
		where TChild : class, IOriginator
	{
		/// <summary>
		/// Raised when children are added/removed to/from the collection.
		/// </summary>
		public event EventHandler OnChildrenChanged;

		private readonly Dictionary<Type, List<TChild>> m_TypeToChildren;
		private readonly IcdOrderedDictionary<int, TChild> m_Children;
		private readonly SafeCriticalSection m_ChildrenSection;
		private readonly PredicateComparer<TChild, int> m_ChildIdComparer;

		#region Properties

		/// <summary>
		/// Gets the number of children in this collection.
		/// </summary>
		public int Count { get { return m_ChildrenSection.Execute(() => m_Children.Count); } }

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TChild this[int id] { get { return GetChild(id); } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractOriginatorCollection()
			: this(Enumerable.Empty<TChild>())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="children"></param>
		protected AbstractOriginatorCollection(IEnumerable<TChild> children)
		{
			m_TypeToChildren = new Dictionary<Type, List<TChild>>();
			m_Children = new IcdOrderedDictionary<int, TChild>();
			m_ChildrenSection = new SafeCriticalSection();
			m_ChildIdComparer = new PredicateComparer<TChild, int>(c => c.Id);

			SetChildren(children);
		}

		#region Methods

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			SetChildren(Enumerable.Empty<TChild>());
		}

		/// <summary>
		/// Clears and sets the children.
		/// </summary>
		/// <returns></returns>
		public void SetChildren(IEnumerable<TChild> children)
		{
			if (children == null)
				throw new ArgumentNullException("children");

			m_ChildrenSection.Enter();

			try
			{
				IEnumerable<TChild> oldChildren = GetChildren();
				RemoveChildren(oldChildren);
				AddChildren(children);
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Removes the given children from the collection.
		/// </summary>
		/// <param name="children"></param>
		public void RemoveChildren(IEnumerable<TChild> children)
		{
			if (children == null)
				throw new ArgumentNullException("children");

			List<TChild> removed = new List<TChild>();

			m_ChildrenSection.Enter();

			try
			{
				foreach (TChild child in children)
				{
					if (child == null)
						throw new InvalidOperationException("Child is null");

					if (!ContainsChild(child.Id))
						continue;

					foreach (Type type in child.GetType().GetAllTypes())
					{
						if (m_TypeToChildren.ContainsKey(type))
							m_TypeToChildren[type].Remove(child);
					}

					m_Children.Remove(child.Id);
					removed.Add(child);
				}
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			if (removed.Count == 0)
				return;

			ChildrenRemoved(removed);

			OnChildrenChanged.Raise(this);
		}

		/// <summary>
		/// Removes the child from the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if the core does not contain the child.</returns>
		public bool RemoveChild(TChild child)
		{
			if (child == null)
				throw new ArgumentNullException("child");

			if (!ContainsChild(child))
				return false;

			RemoveChildren(new[] {child});

			return true;
		}

		/// <summary>
		/// Adds the given children to the collection.
		/// </summary>
		/// <param name="children"></param>
		public void AddChildren(IEnumerable<TChild> children)
		{
			if (children == null)
				throw new ArgumentNullException("children");

			List<TChild> added = new List<TChild>();

			m_ChildrenSection.Enter();

			try
			{
				foreach (TChild child in children)
				{
					if (child == null)
						throw new InvalidOperationException("child");

					if (child.Id <= 0)
						throw new InvalidOperationException(string.Format("Child {0} must have an id greater than 0", child));

					if (ContainsChild(child.Id))
						continue;

					foreach (Type type in child.GetType().GetAllTypes())
					{
						if (!m_TypeToChildren.ContainsKey(type))
							m_TypeToChildren[type] = new List<TChild>();
						m_TypeToChildren[type].AddSorted(child, m_ChildIdComparer);
					}

					m_Children.Add(child.Id, child);
					added.Add(child);
				}
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			if (added.Count == 0)
				return;

			ChildrenAdded(added);

			OnChildrenChanged.Raise(this);
		}

		/// <summary>
		/// Adds the child to the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if a child with the given id already exists.</returns>
		public bool AddChild(TChild child)
		{
			if (child == null)
				throw new ArgumentNullException("child");

			if (ContainsChild(child))
				return false;

			AddChildren(new[] {child});

			return true;
		}

		/// <summary>
		/// Gets all of the children.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TChild> GetChildren()
		{
			return m_ChildrenSection.Execute(() => m_Children.Values.ToArray(m_Children.Count));
		}

		/// <summary>
		/// Gets all of the children of the given type.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		public IEnumerable<TInstance> GetChildren<TInstance>()
			where TInstance : class, TChild
		{
			return GetChildren<TInstance>(c => true);
		}

		/// <summary>
		/// Gets the children matching the given type.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public IEnumerable<TInstance> GetChildren<TInstance>(Func<TInstance, bool> selector)
			where TInstance : class, TChild
		{
			if (selector == null)
				throw new ArgumentNullException("selector");

			m_ChildrenSection.Enter();

			try
			{
				List<TChild> children;
				if (!m_TypeToChildren.TryGetValue(typeof(TInstance), out children))
					return Enumerable.Empty<TInstance>();

				return children.Cast<TInstance>()
				               .Where(selector)
				               .ToArray();
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
		public TChild GetChild(int id)
		{
			m_ChildrenSection.Enter();

			try
			{
				TChild output;
				if (m_Children.TryGetValue(id, out output))
					return output;
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			string message = string.Format("No child found with id {0}", id);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the first child of the given type.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		public TInstance GetChild<TInstance>()
			where TInstance : class, TChild
		{
			m_ChildrenSection.Enter();

			try
			{
				List<TChild> children;
				if (!m_TypeToChildren.TryGetValue(typeof(TInstance), out children))
					throw new InvalidOperationException(string.Format("No {0} of type {1}", typeof(TChild).Name, typeof(TInstance).Name));

				TInstance output;
				if (!children.Cast<TInstance>().TryFirst(out output))
					throw new InvalidOperationException(string.Format("No {0} of type {1}", typeof(TChild).Name, typeof(TInstance).Name));

				return output;

			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
		public TInstance GetChild<TInstance>(int id)
			where TInstance : class, TChild
		{
			TChild child = GetChild(id);

			TInstance instance = child as TInstance;
			if (instance != null)
				return instance;

			string message = string.Format("{0} id {1} is not of type {2}", child.GetType().Name,
			                               id, typeof(TInstance).Name);
			throw new InvalidCastException(message);
		}

		/// <summary>
		/// Returns the first instance of the given type from the given instance ids.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		public TInstanceType GetChild<TInstanceType>(IEnumerable<int> ids)
			where TInstanceType : class, TChild
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			return GetChild<TInstanceType>(ids, i => true);
		}

		/// <summary>
		/// Returns the first instance of the given type from the given instance ids.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TInstanceType GetChild<TInstanceType>(IEnumerable<int> ids, Func<TInstanceType, bool> selector)
			where TInstanceType : class, TChild
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			if (selector == null)
				throw new ArgumentNullException("selector");

			return ids.Select(id => GetChild(id))
			          .OfType<TInstanceType>()
			          .FirstOrDefault(selector);
		}

		/// <summary>
		/// Gets the children with the given ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public IEnumerable<TChild> GetChildren(IEnumerable<int> ids)
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			return GetChildren<TChild>(ids);
		}

		/// <summary>
		/// Gets the children with the given ids, matching the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		public IEnumerable<TInstanceType> GetChildren<TInstanceType>(IEnumerable<int> ids)
			where TInstanceType : class, TChild
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			return GetChildren<TInstanceType>(ids, i => true);
		}

		/// <summary>
		/// Gets the children with the given ids, matching the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public IEnumerable<TInstanceType> GetChildren<TInstanceType>(IEnumerable<int> ids, Func<TInstanceType, bool> selector)
			where TInstanceType : class, TChild
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			if (selector == null)
				throw new ArgumentNullException("selector");

			return ids.Select(id => GetChild(id))
					  .OfType<TInstanceType>()
					  .Where(selector)
					  .ToArray();
		}

		/// <summary>
		/// Gets all of the child ids in the collection.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<int> GetChildrenIds()
		{
			return m_ChildrenSection.Execute(() => m_Children.Keys.ToArray(m_Children.Count));
		}

		/// <summary>
		/// Outputs the child with the given id.
		/// Returns false if there is no child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		public bool TryGetChild(int id, out TChild child)
		{
			m_ChildrenSection.Enter();

			try
			{
				return m_Children.TryGetValue(id, out child);
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Returns true if there is a child with the given id of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ContainsChild<TInstanceType>(int id)
			where TInstanceType : class, TChild
		{
			m_ChildrenSection.Enter();

			try
			{
				TChild child;
				if (!m_Children.TryGetValue(id, out child))
					return false;

				return child is TInstanceType;
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Returns true if at least 1 originator exists of the given type with the given id.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		public bool ContainsChildAny<TInstanceType>(IEnumerable<int> ids)
			where TInstanceType : class, TChild
		{
			m_ChildrenSection.Enter();

			try
			{
				return ids.Where(ContainsChild)
				          .Any(i => m_Children[i] is TInstanceType);
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Returns true if there is a child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ContainsChild(int id)
		{
			return m_ChildrenSection.Execute(() => m_Children.ContainsKey(id));
		}

		/// <summary>
		/// Returns true if there is a child with the given id.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public bool ContainsChild(TChild child)
		{
			if (child == null)
				throw new ArgumentNullException("child");

			return ContainsChild(child.Id);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called when children are added to the collection before any events are raised.
		/// </summary>
		/// <param name="children"></param>
		protected virtual void ChildrenAdded(IEnumerable<TChild> children)
		{
		}

		/// <summary>
		/// Called when children are removed from the collection before any events are raised.
		/// </summary>
		/// <param name="children"></param>
		protected virtual void ChildrenRemoved(IEnumerable<TChild> children)
		{
		}

		#endregion

		#region IEnumerable Methods

		public IEnumerator<TChild> GetEnumerator()
		{
			return GetChildren().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
