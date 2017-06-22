using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings
{
	public abstract class AbstractOriginatorCollection<TChild> : IOriginatorCollection<TChild> where TChild : IOriginator
	{
		public event EventHandler OnChildrenChanged;

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
		{
			m_Children = new Dictionary<int, TChild>();
			m_ChildrenSection = new SafeCriticalSection();
		}

		protected AbstractOriginatorCollection(IEnumerable<TChild> children)
		{
			m_Children = children.ToDictionary(c => c.Id, c => c);
			m_ChildrenSection = new SafeCriticalSection();
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
			return m_ChildrenSection.Execute(() => m_Children.OrderValuesByKey().ToArray());
		}

		/// <summary>
		/// Gets all of the children of the given type.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <returns></returns>
		public IEnumerable<TInstanceType> GetChildren<TInstanceType>()
			where TInstanceType : TChild
		{
			return GetChildren().OfType<TInstanceType>();
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

				string message = string.Format("{0} no child found with id {1}", GetType().Name, id);
				throw new KeyNotFoundException(message);
			}
			finally
			{
				m_ChildrenSection.Leave();
			}
		}

		/// <summary>
		/// Gets the child with the given id.
		/// </summary>
		/// <typeparam name="TInstanceType"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
		public TInstanceType GetChild<TInstanceType>(int id)
			where TInstanceType : TChild
		{
			TChild child = GetChild(id);
			TInstanceType output = (TInstanceType)child;

			if (output != null)
				return output;

			throw new InvalidCastException(string.Format("{0} is not of type {1}", typeof(TChild).Name,
			                                             typeof(TInstanceType).Name));
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
			m_ChildrenSection.Enter();

			try
			{
				if (ContainsChild(child.Id))
					return false;

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
			m_ChildrenSection.Enter();

			try
			{
				if (!ContainsChild(child.Id))
					return false;

				if (!m_Children.Remove(child.Id))
					return false;
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
