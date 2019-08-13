using ICD.Connect.Settings.Originators;
using System.Collections.Generic;

namespace ICD.Connect.Settings.Groups
{
	public interface IGroup<TOriginator> : IGroup
		where TOriginator : IOriginator
	{
		new IEnumerable<TOriginator> GroupItems { get; }
	}

	public interface IGroup : IOriginator
	{
		IEnumerable<IOriginator> GroupItems { get; }
	}
}
