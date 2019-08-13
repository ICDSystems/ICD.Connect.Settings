using System.Collections.Generic;

namespace ICD.Connect.Settings.Groups
{
	public interface IGroupSettings : ISettings
	{
		IEnumerable<int> Ids { get; set; }
	}
}
