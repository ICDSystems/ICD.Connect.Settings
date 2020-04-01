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
}
