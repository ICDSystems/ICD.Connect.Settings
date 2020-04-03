using System;
using System.Collections.Generic;
using System.Globalization;
#if SIMPLSHARP
using Crestron.SimplSharp.CrestronData;
using Crestron.SimplSharp.Reflection;
#else
using System.Data;
using System.Reflection;
#endif
using System.Text;
using ICD.Common.Properties;
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
		/// Gets the property type.
		/// </summary>
		[NotNull]
		public Type PropertyType { get { return Property.PropertyType; } } 

		/// <summary>
		/// If the property type is nullable returns the underlying type, otherwise returns the property type.
		/// </summary>
		[NotNull]
		public Type UnderlyingPropertyType { get { return Nullable.GetUnderlyingType(PropertyType) ?? PropertyType; } }

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		[NotNull]
		public string Name { get { return Property.Name; } }

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
		/// Returns the property as an SQL table creation parameter.
		/// </summary>
		[NotNull]
		public string SqlCreateParam
		{
			get
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
		}

		/// <summary>
		/// Gets the DbType for the property.
		/// </summary>
		/// <returns></returns>
		public DbType DbType { get { return s_TypeToDbType[UnderlyingPropertyType]; } }

		/// <summary>
		/// Gets the SQL type for the property.
		/// </summary>
		/// <returns></returns>
		public string SqlType { get { return s_TypeToSqlType[UnderlyingPropertyType]; } }

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
		public object GetValue(object instance)
		{
			object value = Property.GetValue(instance, null);

			// Hack - Guids are stored as strings
			if (value is Guid)
				value = value.ToString();

			if (value == null)
				value = DBNull.Value;

			return value;
		}

		/// <summary>
		/// Sets the value for the given instance.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="value"></param>
		public void SetValue(object instance, object value)
		{
			if (value == DBNull.Value)
				value = null;

			// Is the property nullable?
			if (value == null)
			{
				if (!PropertyType.CanBeNull())
					throw new ArgumentException(string.Format("Unable to set property {0}.{1} value to NULL",
					                                          Property.DeclaringType.Name, Property.Name));
			}
			else
			{
				// Hack - Guids are stored as strings
				if (UnderlyingPropertyType == typeof(Guid))
					value = new Guid(value as string);

				value = Convert.ChangeType(value, UnderlyingPropertyType, CultureInfo.InvariantCulture);
			}

			Property.SetValue(instance, value, null);
		}

		#endregion
	}
}
