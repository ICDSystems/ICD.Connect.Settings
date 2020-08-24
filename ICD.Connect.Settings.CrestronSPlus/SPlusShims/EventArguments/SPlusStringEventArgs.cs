using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.CrestronSPlus.SPlusShims.EventArguments
{
	public sealed class SPlusStringEventArgs : EventArgs
	{
		[PublicAPI("S+")]
		public string Data { get; set; }

		[PublicAPI("S+")]
		public SPlusStringEventArgs(){}

		public SPlusStringEventArgs(string data)
		{
			Data = data;
		}
	}

}