using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
using Crestron.SimplSharp.Reflection;
#else
using System.Data;
using System.Reflection;
#endif
using System.Text;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.ORM
{
	public sealed class PropertyModel
	{
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

		#region Setup Properties

		[NotNull] public PropertyInfo Property { get; private set; }
		public bool IsPrimaryKey { get; private set; }
		public bool IsForeignKey { get; private set; }
		[CanBeNull] public Type ForeignKeyType { get; private set; }
		public bool IsColumn { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		[NotNull]
		public string Name { get { return Property.Name; } }

		/// <summary>
		/// Gets the property type.
		/// </summary>
		[NotNull]
		public Type PropertyType { get { return Property.PropertyType; } }

		/// <summary>
		/// If the property type is nullable returns the underlying type.
		/// If the property type is GUID returns String.
		/// If the property type is an Enum returns String.
		/// </summary>
		[NotNull]
		public Type StoredPropertyType
		{
			get
			{
				Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;

				if (type == typeof(Guid))
					type = typeof(string);

				if (type.IsEnum)
					type = typeof(string);

				return type;
			}
		}

		/// <summary>
		/// Returns true if this property is a primary key and a numeric type that should Auto-Increment.
		/// </summary>
		public bool AutoIncrements
		{
			get
			{
				if (!IsPrimaryKey)
					return false;

				Type underlyingPropertyType = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
				return underlyingPropertyType.IsNumeric();
			}
		}

		/// <summary>
		/// Gets the DbType for the property.
		/// </summary>
		/// <returns></returns>
		public DbType DbType { get { return s_TypeToDbType[StoredPropertyType]; } }

		/// <summary>
		/// Gets the SQL type for the property.
		/// </summary>
		/// <returns></returns>
		public string SqlType { get { return s_TypeToSqlType[StoredPropertyType]; } }

		/// <summary>
		/// Returns true if the property represents an enumerable.
		/// </summary>
		public bool IsEnumerable { get { return PropertyType.IsAssignableTo(typeof(IEnumerable)); } }

		/// <summary>
		/// If the property is an enumerable, returns the inner generic type. Otherwise, returns the property type.
		/// </summary>
		public Type PropertyOrEnumerableType
		{
			get
			{
				return IsEnumerable ? PropertyType.GetInnerGenericTypes(typeof(IEnumerable<>)).Single() : PropertyType;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		private PropertyModel([NotNull] PropertyInfo property)
		{
			Property = property;
		}

		/// <summary>
		/// Creates a PropertyModel for the given primary key property.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static PropertyModel PrimaryKey([NotNull] PropertyInfo property, [NotNull] PrimaryKeyAttribute attribute)
		{
			return new PropertyModel(property)
			{
				IsPrimaryKey = true,
				IsColumn = true
			};
		}

		/// <summary>
		/// Creates a PropertyModel for the given data property.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static PropertyModel Data([NotNull] PropertyInfo property, [NotNull] DataFieldAttribute attribute)
		{
			return new PropertyModel(property)
			{
				IsColumn = true
			};
		}

		/// <summary>
		/// Creates a PropertyModel for the given foreign key property.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static PropertyModel ForeignKey([NotNull] PropertyInfo property, [NotNull] ForeignKeyAttribute attribute)
		{
			return new PropertyModel(property)
			{
				IsForeignKey = true,
				ForeignKeyType = attribute.Type,

				// Null foreign key type means that this property is NOT an ID column
				IsColumn = attribute.Type != null
			};
		}

		/// <summary>
		/// Creates a PropertyModel for the given anonymous property.
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <returns></returns>
		public static PropertyModel Anonymous(PropertyInfo propertyInfo)
		{
			return new PropertyModel(propertyInfo)
			{
				IsColumn = true
			};
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the value for the given instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public object GetDatabaseValue(object instance)
		{
			object value = Property.GetValue(instance, null);

			// Hack - Guids are stored as strings
			if (value is Guid)
				value = value.ToString();

			return value == null ? DBNull.Value : Convert.ChangeType(value, StoredPropertyType, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Sets the value for the given instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="value"></param>
		public void SetDatabaseValue(object instance, object value)
		{
			if (value == DBNull.Value)
				value = null;

			// Hack - Guids are stored as strings
			if (PropertyType == typeof(Guid))
				value = new Guid((string)value);

			if (value != null)
			{
				Type notNullType = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;

				value = EnumUtils.IsEnumType(notNullType)
					? EnumUtils.ParseStrict(notNullType, (string)value, true)
					: Convert.ChangeType(value, notNullType, CultureInfo.InvariantCulture);
			}

			Property.SetValue(instance, value, null);
		}

		/// <summary>
		/// Returns the property as an SQL table creation parameter.
		/// </summary>
		[NotNull]
		public string GetSqlCreateParam()
		{
			// Name
			StringBuilder builder = new StringBuilder(Name);
			{
				// Type
				builder.AppendFormat(" {0}", SqlType);

				// Primary key
				if (IsPrimaryKey)
					builder.Append(" PRIMARY KEY");

				// Auto-increment
				if (AutoIncrements)
					builder.Append(" AUTOINCREMENT");
			}
			return builder.ToString();
		}

		/// <summary>
		/// Returns the constraint information for this property.
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public string GetSqlConstraintParam()
		{
			if (IsForeignKey && ForeignKeyType != null)
			{
				TypeModel parent = TypeModel.Get(ForeignKeyType);
				return string.Format("FOREIGN KEY({0}) REFERENCES {1}({2}) ON DELETE CASCADE", Name,
				                     parent.TableName, parent.PrimaryKey.Name);
			}

			return null;
		}

		#endregion
	}
}
