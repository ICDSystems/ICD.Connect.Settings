using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.ORM
{
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class DataFieldAttribute : Attribute
	{
	}

	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class PrimaryKeyAttribute : Attribute
	{
	}

	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ForeignKeyAttribute : Attribute
	{
		/// <summary>
		/// Gets the type that this foreign key points to.
		/// </summary>
		[CanBeNull]
		public Type Type { get; private set; }

		/// <summary>
		/// The foreign type is inferred from the property.
		/// </summary>
		public ForeignKeyAttribute()
			: this(null)
		{
		}

		/// <summary>
		/// The property is an ID pointing to the given type.
		/// </summary>
		/// <param name="type"></param>
		public ForeignKeyAttribute([CanBeNull] Type type)
		{
			Type = type;
		}
	}
}
