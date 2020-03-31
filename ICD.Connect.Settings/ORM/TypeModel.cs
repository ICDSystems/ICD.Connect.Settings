using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
				{typeof(int), "int"},
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

		private string m_PrimaryKeyName;
		private IcdOrderedDictionary<string, PropertyInfo> m_Props;
		private readonly Type m_Type;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static TypeModel()
		{
			s_TypeModels = new Dictionary<Type, TypeModel>();
		}

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

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		private TypeModel([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			m_Type = type;

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
			// Get the data properties
			IEnumerable<PropertyInfo> properties = type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetProperties()
				.Where(p => p.GetCustomAttributes(typeof(DataFieldAttribute), false).Length > 0);

			m_Props = new IcdOrderedDictionary<string, PropertyInfo>();
			m_Props.AddRange(properties, p => p.Name);

			// Get the primary key property
			PropertyInfo pkProp = type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetProperties()
				.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0);

			m_PrimaryKeyName = pkProp.Name;
			m_Props.Add(m_PrimaryKeyName, pkProp);
		}

		/// <summary>
		/// The Type represents an anonymous class
		/// </summary>
		/// <param name="type"></param>
		private void PopulateAnonymous(Type type)
		{
			// Get the data fields
			IEnumerable<PropertyInfo> properties = type
#if SIMPLSHARP
				.GetCType()
#endif
				.GetProperties();

			m_Props = new IcdOrderedDictionary<string, PropertyInfo>();
			m_Props.AddRange(properties, p => p.Name);
		}

		public static TypeModel Get(Type type)
		{
			return s_TypeModels.GetOrAddNew(type, () => new TypeModel(type));
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the DbType for the given column.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public DbType GetPropertyType(object instance, string columnName)
		{
			PropertyInfo pi = m_Props[columnName];
			object value = pi.GetValue(instance, null);

			// Can't be ternary because of CType nonsense
			Type type = pi.PropertyType;
			if (value != null)
				type = value.GetType();

			return GetPropertyType(type);
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

		public string GetDelimitedSafeParamList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Keys.Select(k => string.Format("@{0}", k)).ToArray());
		}

		public string GetDelimitedSafeFieldList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Keys.Select(k => string.Format("[{0}]", k)).ToArray());
		}

		public string GetDelimitedSafeSetList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Keys.Select(k => string.Format("[{0}] = @{0}", k)).ToArray());
		}

		public string GetDelimitedCreateParamList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Select(kvp => GetCreateParam(kvp)).ToArray());
		}

		#endregion

		#region Private Methods

		private static string GetCreateParam(KeyValuePair<string, PropertyInfo> kvp)
		{
			Type underlyingPropertyType = Nullable.GetUnderlyingType(kvp.Value.PropertyType) ?? kvp.Value.PropertyType;
			return string.Format("{0} {1}", kvp.Key, s_TypeToSqlType[underlyingPropertyType]);
		}

		/// <summary>
		/// Gets the DbType for the given C# Type.
		/// </summary>
		/// <param name="propertyType"></param>
		/// <returns></returns>
		private DbType GetPropertyType(Type propertyType)
		{
			Type underlyingPropertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
			return s_TypeToDbType[underlyingPropertyType];
		}

		#endregion
	}
}
