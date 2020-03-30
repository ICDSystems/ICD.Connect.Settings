using System;

namespace ICD.Connect.Settings.ORM
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class DataFieldAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class PrimaryKeyAttribute : Attribute
	{
	}
}
