using ICD.Connect.Settings.Originators;
using System.Collections.Generic;

namespace ICD.Connect.Settings.Groups
{
	public interface IGroup<TOriginator> where TOriginator : IOriginator
	{
		IEnumerable<TOriginator> GroupItems { get; }
	}
}
