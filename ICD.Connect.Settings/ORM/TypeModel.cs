using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ICD.Common.Utils.Collections;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
using Crestron.SimplSharp.Reflection;
#else
using System.Data;
using System.Reflection;
#endif
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.ORM
{
	public sealed class TypeModel
	{
		private static readonly Dictionary<Type, TypeModel> s_TypeModels;

		private static readonly Dictionary<Type, string> s_TypeToSqlType =
			new Dictionary<Type, string>
			{
				{typeof(short), "smallint"},
				{typeof(int), "INTEGER"},
				{typeof(long), "bigint"},
				{typeof(string), "NVARCHAR"},
				{typeof(byte), "byte"},
				{typeof(byte[]), "varbinary"},
				{typeof(Guid), "NVARCHAR"},
				{typeof(TimeSpan), "time"},
				{typeof(decimal), "money"},
				{typeof(bool), "bit"},
				{typeof(DateTime), "datetime"},
				{typeof(double), "float"},
				{typeof(float), "single"}
			};

		private static readonly Dictionary<Type, DbType> s_TypeToDbType =
			new Dictionary<Type, DbType>
			{
				{typeof(short), DbType.Int16},
				{typeof(int), DbType.Int32},
				{typeof(long), DbType.Int64},
				{typeof(string), DbType.String},
				{typeof(byte), DbType.Binary},
				{typeof(byte[]), DbType.Binary},
				{typeof(Guid), DbType.String},
				{typeof(TimeSpan), DbType.Time},
				{typeof(decimal), DbType.Currency},
				{typeof(bool), DbType.Boolean},
				{typeof(DateTime), DbType.DateTime},
				{typeof(double), DbType.Double},
				{typeof(float), DbType.Single}
			};

		private readonly IcdOrderedDictionary<string, PropertyInfo> m_Props;
		private readonly Type m_Type;

		private string m_PrimaryKeyName;

		#region Properties

		public string PrimaryKeyName { get { return m_PrimaryKeyName; } }

		public string TableName
		{
			get
			{
				string s = m_Type.Name;
				char lastchar = s[s.Length - 1];

				if (lastchar == 'y')
					return s.Remove(s.Length - 1, 1) + "ies";

				if (lastchar == 's')
					return s;

				return s + "s";
			}
		}

		/// <summary>
		/// Returns true if the primary key is a numeric type that should Auto-Increment.
		/// </summary>
		public bool AutoIncrements
		{
			get
			{
				if (string.IsNullOrEmpty(m_PrimaryKeyName))
					throw new InvalidOperationException("TypeModel does not represent a class with a primary key attribute");

				PropertyInfo prop = m_Props[m_PrimaryKeyName];
				Type underlyingPropertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

				return underlyingPropertyType.IsNumeric();
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
			m_Props = new IcdOrderedDictionary<string, PropertyInfo>(new ColumnComparer(this));

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
			PropertyInfo pkProp = type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetProperties()
				.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0);

			m_PrimaryKeyName = pkProp.Name;
			m_Props.Add(m_PrimaryKeyName, pkProp);

			// Get the data properties
			IEnumerable<PropertyInfo> properties = type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetProperties()
				.Where(p => p.GetCustomAttributes(typeof(DataFieldAttribute), false).Length > 0);
			m_Props.AddRange(properties, p => p.Name);
		}

		/// <summary>
		/// The Type represents an anonymous class
		/// </summary>
		/// <param name="type"></param>
		private void PopulateAnonymous(Type type)
		{
			// Get the data properties
			IEnumerable<PropertyInfo> properties = type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetProperties();
			m_Props.AddRange(properties, p => p.Name);
		}

		public static TypeModel Get(Type type)
		{
			return s_TypeModels.GetOrAddNew(type, () => new TypeModel(type));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the property info for the given column.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public PropertyInfo GetProperty(string columnName)
		{
			return m_Props[columnName];
		}

		/// <summary>
		/// Gets the DbType for the given column.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public DbType GetPropertyDbType(string columnName)
		{
			PropertyInfo pi = m_Props[columnName];
			Type underlyingPropertyType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
			return s_TypeToDbType[underlyingPropertyType];
		}

		/// <summary>
		/// Gets the SQL type for the given column.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public string GetPropertySqlType(string columnName)
		{
			PropertyInfo pi = m_Props[columnName];
			Type underlyingPropertyType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
			return s_TypeToSqlType[underlyingPropertyType];
		}

		/// <summary>
		/// Gets the database value for the given property name on the given instance.
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public object GetPropertyValue(object instance, string columnName)
		{
			PropertyInfo pi = m_Props[columnName];
			object value = pi.GetValue(instance, null);

			// Hack - Guids are stored as strings
			if (value is Guid)
				value = value.ToString();

			return value;
		}

		/// <summary>
		/// Sets the database value for the given property name on the given instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="columnName"></param>
		/// <param name="value"></param>
		public void SetPropertyValue(object instance, string columnName, object value)
		{
			PropertyInfo pi = m_Props[columnName];

			// Is the property nullable?
			Type underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

			// Hack - Guids are stored as strings
			if (underlying == typeof(Guid))
				value = new Guid(value as string);

			value = Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
			pi.SetValue(instance, value, null);
		}

		public IEnumerable<string> GetPropertyNames()
		{
			return m_Props.Keys;
		}

		public string GetDelimitedCreateParamList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Keys.Select(columnName => GetCreateParam(columnName)).ToArray());
		}

		#endregion

		#region Private Methods

		private string GetCreateParam(string columnName)
		{
			// Name
			StringBuilder builder = new StringBuilder(columnName);
			{
				// Type
				builder.AppendFormat(" {0}", GetPropertySqlType(columnName));

				// Primary key
				if (columnName == m_PrimaryKeyName)
					builder.Append(" PRIMARY KEY");

				// Auto-increment
				if (columnName == m_PrimaryKeyName && AutoIncrements)
					builder.Append(" AUTOINCREMENT");
			}
			return builder.ToString();
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
				bool xIsPrimary = x == m_TypeModel.m_PrimaryKeyName;
				bool yIsPrimary = y == m_TypeModel.m_PrimaryKeyName;

				if (xIsPrimary && !yIsPrimary)
					return -1;
				if (yIsPrimary && !xIsPrimary)
					return 1;

				return string.Compare(x, y, StringComparison.Ordinal);
			}
		}
	}
}
