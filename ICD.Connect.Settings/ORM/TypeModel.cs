using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Collections;
#if SIMPLSHARP
using Crestron.SimplSharp.Reflection;
#else
using System.Reflection;
#endif
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.ORM
{
	public sealed class TypeModel
	{
		private static readonly Dictionary<Type, TypeModel> s_TypeModels;

		private readonly IcdSortedDictionary<string, PropertyModel> m_Props;
		private readonly Type m_Type;

		private PropertyModel m_PrimaryKey;

		#region Properties

		/// <summary>
		/// Returns the primary key.
		/// </summary>
		[CanBeNull]
		public PropertyModel PrimaryKey { get { return m_PrimaryKey; } }

		/// <summary>
		/// Returns the name of the table for the wrapped type.
		/// </summary>
		public string TableName
		{
			get
			{
				string s = m_Type.Name;
				char lastChar = s[s.Length - 1];

				switch (lastChar)
				{
					case 'y':
						return s.Remove(s.Length - 1, 1) + "ies";
					case 's':
						return s;
					default:
						return s + "s";
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Static constructor.
		/// </summary>
		static TypeModel()
		{
			s_TypeModels = new Dictionary<Type, TypeModel>();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		private TypeModel([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			m_Type = type;
			m_Props = new IcdSortedDictionary<string, PropertyModel>(new ColumnComparer(this));

			if (m_Type.IsAnonymous())
				PopulateAnonymous(m_Type);
			else
				Populate(m_Type);
		}

		/// <summary>
		/// The Type represents a "real" class that has been built for ORM.
		/// </summary>
		/// <param name="type"></param>
		private void Populate(Type type)
		{
			// Get the primary key property
			m_PrimaryKey = GetPrimaryKey(type);
			m_Props.Add(m_PrimaryKey.Name, m_PrimaryKey);

			// Get the data properties
			IEnumerable<PropertyModel> dataProperties = GetData(type);
			m_Props.AddRange(dataProperties, p => p.Name);

			// Get the foreign key properties
			IEnumerable<PropertyModel> foreignProperties = GetForeignKeys(type);
			m_Props.AddRange(foreignProperties, p => p.Name);
		}

		/// <summary>
		/// The Type represents an anonymous class
		/// </summary>
		/// <param name="type"></param>
		private void PopulateAnonymous(Type type)
		{
			// Get the data properties
			IEnumerable<PropertyModel> dataProperties = GetAnonymousProperties(type);
			m_Props.AddRange(dataProperties, p => p.Name);
		}

		/// <summary>
		/// Lazy-loads the TypeModel for the given Type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypeModel Get(Type type)
		{
			return s_TypeModels.GetOrAddNew(type, () => new TypeModel(type));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the property model for the given column.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public PropertyModel GetProperty(string columnName)
		{
			return m_Props[columnName];
		}

		/// <summary>
		/// Gets all of the properties, including properties that don't have columns.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PropertyModel> GetProperties()
		{
			return m_Props.Values;
		}

		/// <summary>
		/// Gets all of the tracked columns for the wrapped type.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PropertyModel> GetColumns()
		{
			return m_Props.Values.Where(p => p.IsColumn);
		}

		/// <summary>
		/// Gets the SQL parameters string for creating a new table.
		/// </summary>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public string GetDelimitedCreateParamList(string delimiter)
		{
			IEnumerable<string> columns = GetColumns().Select(p => p.GetSqlCreateParam());
			IEnumerable<string> constraints = GetColumns().Select(p => p.GetSqlConstraintParam()).Where(p => p != null);

			return string.Join(delimiter, columns.Concat(constraints).ToArray());
		}

		/// <summary>
		/// Gets the list of SQL column names.
		/// </summary>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public string GetDelimitedColumnNames(string delimiter)
		{
			return string.Join(delimiter, GetColumns().Select(c => c.Name).ToArray());
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the property model for the primary key for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		private static PropertyModel GetPrimaryKey([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			try
			{
				return GetProperties(type)
					.Select(p =>
					        {
						        PrimaryKeyAttribute attribute =
							        p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true)
							         .OfType<PrimaryKeyAttribute>()
							         .SingleOrDefault();

						        return attribute == null ? null : PropertyModel.PrimaryKey(p, attribute);
					        })
					.Single(p => p != null);
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException(type.Name + " has no primary key", "type");
			}
		}

		/// <summary>
		/// Gets the property models for the data properties for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		private static IEnumerable<PropertyModel> GetData([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetProperties(type)
				.Select(p =>
				        {
					        DataFieldAttribute attribute =
						        p.GetCustomAttributes(typeof(DataFieldAttribute), true)
						         .OfType<DataFieldAttribute>()
						         .SingleOrDefault();

					        return attribute == null ? null : PropertyModel.Data(p, attribute);
				        })
				.Where(p => p != null);
		}

		/// <summary>
		/// Gets the property models for the foreign key properties for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		private static IEnumerable<PropertyModel> GetForeignKeys([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetProperties(type)
				.Select(p =>
				        {
					        ForeignKeyAttribute attribute =
						        p.GetCustomAttributes(typeof(ForeignKeyAttribute), true)
						         .OfType<ForeignKeyAttribute>()
						         .SingleOrDefault();

					        return attribute == null ? null : PropertyModel.ForeignKey(p, attribute);
				        })
				.Where(p => p != null);
		}

		private static IEnumerable<PropertyModel> GetAnonymousProperties([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetProperties(type).Select(p => PropertyModel.Anonymous(p));
		}

		/// <summary>
		/// Returns the properties for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[NotNull]
		private static IEnumerable<PropertyInfo> GetProperties([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return
				type
#if SIMPLSHARP
					.GetCType()
#endif
					.GetProperties();
		}

		#endregion

		/// <summary>
		/// Sorts private key to the front, otherwise alphabetical.
		/// </summary>
		private sealed class ColumnComparer : IComparer<string>
		{
			private readonly TypeModel m_TypeModel;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="typeModel"></param>
			public ColumnComparer(TypeModel typeModel)
			{
				m_TypeModel = typeModel;
			}

			public int Compare(string x, string y)
			{
				string primaryKeyName = m_TypeModel.PrimaryKey == null ? null : m_TypeModel.PrimaryKey.Name;

				bool xIsPrimary = x == primaryKeyName;
				bool yIsPrimary = y == primaryKeyName;

				if (xIsPrimary && !yIsPrimary)
					return -1;
				if (yIsPrimary && !xIsPrimary)
					return 1;

				return string.Compare(x, y, StringComparison.Ordinal);
			}
		}
	}
}
