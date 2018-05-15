using System;

namespace ICD.Connect.Settings.SPlusShims
{
	public interface ISPlusShim : IDisposable
	{
		 string Location { get; set; }
	}
}