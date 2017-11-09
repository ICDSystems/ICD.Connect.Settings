using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings
{
	public abstract class AbstractOriginatorCollection<TChild> : IOriginatorCollection<TChild>
		where TChild : IOriginator
	{
		public event EventHandler OnChildrenChanged;

		private readonly Dictionary<Type, IcdHashSet<TChild>> m_TypeToChildren; 
		private readonly Dictionary<int, TChild> m_Children;
		private readonly SafeCriticalSection m_ChildrenSection;

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
			m_TypeToChildren = new Dictionary<Type, IcdHashSet<TChild>>();
			m_Children = new Dictionary<int, TChild>();
			m_ChildrenSection = new SafeCriticalSection();

			SetChildren(children);
		}

		#region Methods

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			m_ChildrenSection.Enter();

			try
			{
				if (Count == 0)
					return;

				m_TypeToChildren.Clear();
				m_Children.Clear();
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			OnChildrenChanged.Raise(this);
		}

		/// <summary>
		/// Clears and sets the children.
		/// </summary>
		/// <returns></returns>
		public void SetChildren(IEnumerable<TChild> children)
		{
			m_ChildrenSection.Enter();

			try
			{
				Clear();
				children.ForEach(c => AddChild(c));
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Gets all of the children.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TChild> GetChildren()
		{
			return m_ChildrenSection.Execute(() => m_Children.Values.ToArray());
		}

		/// <summary>
		/// Gets all of the children of the given type.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		public IEnumerable<TInstance> GetChildren<TInstance>()
			where TInstance : TChild
		{
			m_ChildrenSection.Enter();

			try
			{
				IcdHashSet<TChild> children;
				if (!m_TypeToChildren.TryGetValue(typeof(TInstance), out children))
					return Enumerable.Empty<TInstance>();

				return children.OfType<TInstance>().ToArray();
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

			string message = string.Format("{0} no child found with id {1}", GetType().Name, id);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the first child of the given type.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		public TInstance GetChild<TInstance>()
			where TInstance : TChild
		{
			m_ChildrenSection.Enter();

			try
			{
				IcdHashSet<TChild> children;
				m_TypeToChildren.TryGetValue(typeof(TInstance), out children);

				if (children == null || children.Count == 0)
					throw new InvalidOperationException(string.Format("No {0} of type {1}", typeof(TChild).Name,
					                                                  typeof(TInstance).Name));

				return (TInstance)children.First();
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
			where TInstance : TChild
		{
			TChild child = GetChild(id);

			if (child.GetType().IsAssignableTo(typeof(TInstance)))
				return (TInstance)child;

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
			where TInstanceType : TChild
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
			where TInstanceType : TChild
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
			return GetChildren<TChild>(ids);
		}

		/// <summary>
		/// Gets the children with the given ids, matching the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="ids"></param>
		/// <returns></returns>
		public IEnumerable<TInstanceType> GetChildren<TInstanceType>(IEnumerable<int> ids)
			where TInstanceType : TChild
		{
			if (ids == null)
				throw new ArgumentNullException("ids");

			return ids.Select(id => GetChild(id))
			          .OfType<TInstanceType>()
			          .ToArray();
		}

		/// <summary>
		/// Gets all of the child ids in the collection.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<int> GetChildrenIds()
		{
			return m_ChildrenSection.Execute(() => m_Children.Keys);
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
		/// Returns true if there is a child with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ContainsChild(int id)
		{
			return m_ChildrenSection.Execute(() => m_Children.ContainsKey(id));
		}

		/// <summary>
		/// Adds the child to the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if a child with the given id already exists.</returns>
		public bool AddChild(TChild child)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (child == null)
				throw new ArgumentNullException("child");

			m_ChildrenSection.Enter();

			try
			{
				if (ContainsChild(child.Id))
					return false;

				foreach (Type type in child.GetType().GetAllTypes())
				{
					if (!m_TypeToChildren.ContainsKey(type))
						m_TypeToChildren[type] = new IcdHashSet<TChild>();
					m_TypeToChildren[type].Add(child);
				}

				m_Children.Add(child.Id, child);
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			OnChildrenChanged.Raise(this);
			return true;
		}

		/// <summary>
		/// Removes the child from the core.
		/// </summary>
		/// <param name="child"></param>
		/// <returns>False if the core does not contain the child.</returns>
		public bool RemoveChild(TChild child)
		{
// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (child == null)
				throw new ArgumentNullException("child");

			m_ChildrenSection.Enter();

			try
			{
				if (!ContainsChild(child.Id))
					return false;

				if (!m_Children.Remove(child.Id))
					return false;

				foreach (Type type in child.GetType().GetAllTypes())
				{
					if (m_TypeToChildren.ContainsKey(type))
						m_TypeToChildren[type].Remove(child);
				}
			}
			finally
			{
				m_ChildrenSection.Leave();
			}

			OnChildrenChanged.Raise(this);
			return true;
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
