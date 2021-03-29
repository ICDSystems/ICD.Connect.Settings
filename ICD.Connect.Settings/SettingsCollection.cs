using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Settings
{
	/// <summary>
	/// Provides a collection of settings
	/// </summary>
	public sealed class SettingsCollection : ICollection<ISettings>
	{
		public event EventHandler<GenericEventArgs<ISettings>> OnItemAdded;
		public event EventHandler<GenericEventArgs<ISettings>> OnItemRemoved;

		private readonly IcdSortedDictionary<int, ISettings> m_Collection;
		private readonly SafeCriticalSection m_CollectionSection;

		#region Properties

		/// <summary>
		/// The number of stored settings.
		/// </summary>
		public int Count { get { return m_CollectionSection.Execute(() => m_Collection.Count); } }

		public bool IsReadOnly { get { return false; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsCollection()
			: this(Enumerable.Empty<ISettings>())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="settings"></param>
		public SettingsCollection([NotNull] IEnumerable<ISettings> settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			m_Collection = new IcdSortedDictionary<int, ISettings>();
			m_CollectionSection = new SafeCriticalSection();

			AddRange(settings);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Writes the collection to XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="childElement"></param>
		public void ToXml([NotNull] IcdXmlTextWriter writer, string element, string childElement)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			m_CollectionSection.Enter();

			try
			{
				writer.WriteStartElement(element);
				{
					foreach (ISettings item in m_Collection.Values)
						item.ToXml(writer, childElement);
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
			return m_CollectionSection.Execute(() => m_Collection.Values.ToList()).GetEnumerator();
		}

		/// <summary>
		/// Adds the item to the collection unless the id already exists
		/// </summary>
		/// <param name="item"></param>
		public bool Add([NotNull] ISettings item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			m_CollectionSection.Enter();

			try
			{
				if (m_Collection.ContainsKey(item.Id))
					return false;

				m_Collection[item.Id] = item;
			}
			finally
			{
				m_CollectionSection.Leave();
			}

			OnItemAdded.Raise(this, new GenericEventArgs<ISettings>(item));
			return true;
		}

		/// <summary>
		/// Adds the items to the collection.
		/// </summary>
		/// <param name="items"></param>
		public void AddRange([NotNull] IEnumerable<ISettings> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			foreach (ISettings item in items)
				Add(item);
		}

		/// <summary>
		/// Clears the collection and sets the items.
		/// </summary>
		/// <param name="items"></param>
		public void SetRange([NotNull] IEnumerable<ISettings> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			Clear();
			AddRange(items);
		}

		/// <summary>
		/// Returns the settings instance with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
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
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			foreach (ISettings item in m_CollectionSection.Execute(() => m_Collection.Values.ToArray(m_Collection.Count)))
				Remove(item);
		}

		/// <summary>
		/// Returns true if the collection contains the item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains([NotNull] ISettings item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

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
		public void CopyTo([NotNull] ISettings[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			m_CollectionSection.Enter();

			try
			{
				m_Collection.Values
				            .ToArray(m_Collection.Count)
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
		public bool Remove([NotNull] ISettings item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

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

			OnItemRemoved.Raise(this, new GenericEventArgs<ISettings>(item));
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
