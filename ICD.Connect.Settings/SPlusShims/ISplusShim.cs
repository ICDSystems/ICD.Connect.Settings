using System;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Settings.SPlusShims
{
	public interface ISPlusShim : IDisposable, IConsoleNode
	{
		 string Location { get; set; }
	}
}