using System.Collections.Generic;

namespace ICD.Connect.Settings.Groups
{
	public interface IGroupSettings : ISettings
	{
		void SetIds(IEnumerable<int> value);
		IEnumerable<int> GetIds();
	}
}
