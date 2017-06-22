using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Services;
using ICD.Common.Services.Logging;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Provides a collection of settings
	/// </summary>
	public sealed class SettingsCollection : ICollection<ISettings>
	{
		public event EventHandler OnItemAdded;
		public event EventHandler OnItemRemoved;

		private readonly Dictionary<int, ISettings> m_Collection;
		private readonly SafeCriticalSection m_CollectionSection;

		#region Properties

		/// <summary>
		/// The number of stored settings.
		/// </summary>
		public int Count { get { return m_CollectionSection.Execute(() => m_Collection.Count); } }

		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// Returns the settings instance with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ISettings this[int id] { get { return GetById(id); } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsCollection()
		{
			m_Collection = new Dictionary<int, ISettings>();
			m_CollectionSection = new SafeCriticalSection();
		}

		public SettingsCollection(IEnumerable<ISettings> settings) : this()
		{
			AddRange(settings);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Writes the collection to XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		public void ToXml(IcdXmlTextWriter writer, string element)
		{
			m_CollectionSection.Enter();

			try
			{
				writer.WriteStartElement(element);
				{
					foreach (ISettings item in m_Collection.OrderValuesByKey())
						item.ToXml(writer);
				}
				writer.WriteEndElement();
			}
			finally
			{
				m_CollectionSection.Leave();
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ISettings> GetEnumerator()
		{
			return m_CollectionSection.Execute(() => m_Collection.OrderValuesByKey().ToList()).GetEnumerator();
		}

		/// <summary>
		/// Adds the item to the collection unless the id already exists
		/// </summary>
		/// <param name="item"></param>
		public bool Add(ISettings item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			m_CollectionSection.Enter();

			try
			{
				if (m_Collection.ContainsKey(item.Id))
				{
					if (item != m_Collection[item.Id])
					{
						ServiceProvider.TryGetService<ILoggerService>().AddEntry(
						                                                         eSeverity.Warning,
						                                                         "{0} already exists for id {1}, tried adding {2}",
						                                                         m_Collection[item.Id].GetType().Name, item.Id,
						                                                         item.GetType().Name);
					}
					return false;
				}

				m_Collection[item.Id] = item;
			}
			finally
			{
				m_CollectionSection.Leave();
			}

			OnItemAdded.Raise(this);
			return true;
		}

		/// <summary>
		/// Adds the items to the collection.
		/// </summary>
		/// <param name="items"></param>
		public void AddRange(IEnumerable<ISettings> items)
		{
			m_CollectionSection.Enter();

			try
			{
				foreach (ISettings item in items)
					Add(item);
			}
			finally
			{
				m_CollectionSection.Leave();
			}
		}

		/// <summary>
		/// Clears the collection and sets the items.
		/// </summary>
		/// <param name="items"></param>
		public void SetRange(IEnumerable<ISettings> items)
		{
			m_CollectionSection.Enter();

			try
			{
				Clear();
				AddRange(items);
			}
			finally
			{
				m_CollectionSection.Leave();
			}
		}

		/// <summary>
		/// Returns the settings instance with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ISettings GetById(int id)
		{
			m_CollectionSection.Enter();

			try
			{
				ISettings output;
				if (m_Collection.TryGetValue(id, out output))
					return output;

				string message = string.Format("{0} contains no item with id {1}", GetType().Name, id);
				throw new KeyNotFoundException(message);
			}
			finally
			{
				m_CollectionSection.Leave();
			}
		}

		/// <summary>
		/// Returns an unused id.
		/// </summary>
		/// <returns></returns>
		public int GetNewId()
		{
			return m_CollectionSection.Execute(() => MathUtils.GetNewId(m_Collection.Keys));
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			m_CollectionSection.Enter();

			try
			{
				if (m_Collection.Count == 0)
					return;

				m_Collection.Clear();
			}
			finally
			{
				m_CollectionSection.Leave();
			}

			OnItemRemoved.Raise(this);
		}

		/// <summary>
		/// Returns true if the collection contains the item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(ISettings item)
		{
			return ContainsId(item.Id);
		}

		/// <summary>
		/// Returns true if the collection contains an item with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ContainsId(int id)
		{
			return m_CollectionSection.Execute(() => m_Collection.ContainsKey(id));
		}

		/// <summary>
		/// Copies the collection to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(ISettings[] array, int arrayIndex)
		{
			m_CollectionSection.Enter();

			try
			{
				m_Collection.OrderValuesByKey()
				            .ToArray()
				            .CopyTo(array, arrayIndex);
			}
			finally
			{
				m_CollectionSection.Leave();
			}
		}

		/// <summary>
		/// Removes the item with the given key.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(ISettings item)
		{
			m_CollectionSection.Enter();

			try
			{
				if (!m_Collection.Remove(item.Id))
					return false;
			}
			finally
			{
				m_CollectionSection.Leave();
			}

			OnItemRemoved.Raise(this);
			return true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds the item to the collection.
		/// </summary>
		/// <param name="item"></param>
		void ICollection<ISettings>.Add(ISettings item)
		{
			Add(item);
		}

		#endregion
	}
}
