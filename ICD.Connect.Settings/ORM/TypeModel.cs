using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

		private static readonly Dictionary<Type, string> s_TypeToSqlType =
			new Dictionary<Type, string>
			{
				{typeof(short), "smallint"},
				{typeof(short?), "smallint"},
				{typeof(int), "int"},
				{typeof(int?), "int"},
				{typeof(long), "bigint"},
				{typeof(long?), "bigint"},
				{typeof(string), "NVARCHAR"},
				//{typeof(Xml), "Xml"},
				{typeof(byte), "binary"},
				{typeof(byte?), "binary"},
				{typeof(byte[]), "varbinary"},
				{typeof(Guid), "uniqueidentifier"},
				{typeof(Guid?), "uniqueidentifier"},
				{typeof(TimeSpan), "time"},
				{typeof(TimeSpan?), "time"},
				{typeof(decimal), "money"},
				{typeof(decimal?), "money"},
				{typeof(bool), "bit"},
				{typeof(bool?), "but"},
				{typeof(DateTime), "datetime"},
				{typeof(DateTime?), "datetime"},
				{typeof(double), "float"},
				{typeof(double?), "float"},
				{typeof(float), "float"},
				{typeof(float?), "float"},
				{typeof(char[]), "nchar"}
			};

		private readonly string m_PrimaryKeyName;
		private readonly Dictionary<string, PropertyInfo> m_Props;
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

		public string InsertStatement
		{
			get
			{
				return string.Format("INSERT INTO [{0}] ({1}) VALUES ({2})",
				                     TableName,
				                     GetDelimitedSafeFieldList(", "),
				                     GetDelimitedSafeParamList(", "));
			}
		}

		public string UpdateStatement
		{
			get
			{
				return string.Format("UPDATE [{0}] SET {1} WHERE [{2}] = @{2}",
				                     TableName,
				                     GetDelimitedSafeSetList(", "),
				                     m_PrimaryKeyName);
			}
		}

		public string DeleteStatement
		{
			get
			{
				return string.Format("DELETE [{0}] WHERE [{1}] = @{1}",
				                     TableName,
				                     m_PrimaryKeyName);
			}
		}

		public string SelectStatement
		{
			get
			{
				return string.Format("SELECT [{0}], {1} FROM [{2}]",
				                     m_PrimaryKeyName,
				                     GetDelimitedSafeFieldList(", "),
				                     TableName);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		public TypeModel([NotNull] Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			m_Type = type;

			m_Props = m_Type
#if SIMPLSHARP
				.GetCType()
#endif
			          .GetProperties()
			          .Where(p => p.GetCustomAttributes(typeof(DataFieldAttribute), false).Length > 0)
			          .ToDictionary(p => p.Name);

			PropertyInfo pkProp = m_Type
#if SIMPLSHARP
				.GetCType()
#endif
			                      .GetProperties()
			                      .Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0);

			m_PrimaryKeyName = pkProp.Name;
			m_Props.Add(m_PrimaryKeyName, pkProp);
		}

		public static TypeModel Get(Type type)
		{
			return s_TypeModels.GetOrAddNew(type, () => new TypeModel(type));
		}

		#region Methods

		public void SetProperty(object instance, string columnName, object value)
		{
			PropertyInfo pi = m_Props[columnName];

			// Is the property nullable?
			Type underlying = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;

			// Hack - Guids are stored as blobs
			if (underlying == typeof(Guid))
				value = new Guid(value as byte[]);

			value = Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
			pi.SetValue(instance, value, null);
		}

		public PropertyInfo GetProperty(string name)
		{
			return m_Props[name];
		}

		public IEnumerable<string> GetPropertyNames()
		{
			return m_Props.Keys;
		}

		public string GetDelimitedSafeParamList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Select(k => string.Format("@{0}", k)).ToArray());
		}

		public string GetDelimitedSafeFieldList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Select(k => string.Format("[{0}]", k)).ToArray());
		}

		public string GetDelimitedSafeSetList(string delimiter)
		{
			return string.Join(delimiter, m_Props.Select(k => string.Format("[{0}] = @{0}", k)).ToArray());
		}

		public string GetDelimitedCreateParamList(string delimeter)
		{
			return string.Join(delimeter, m_Props.Select(k => GetCreateParam(k.Value)).ToArray());
		}

		private string GetCreateParam(PropertyInfo value)
		{
			return string.Format("{0} {1}", value.Name, s_TypeToSqlType[value.PropertyType]);
		}

		#endregion
	}
}
