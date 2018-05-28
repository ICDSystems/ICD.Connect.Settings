using System;
using ICD.Common.Properties;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Settings.SPlusShims
{
	public interface ISPlusShim : IDisposable, IConsoleNode
	{
		/// <summary>
		/// Location in the SimplWindows program of the S+ Module
		/// Used to aid debugging
		/// </summary>
		[PublicAPI("S+")]
		string Location { get; set; }

		/// <summary>
		/// Programmer specified name of the module
		/// Used to aid debugging
		/// </summary>
		[PublicAPI("S+")]
		string Name { get; set; }
	}

}