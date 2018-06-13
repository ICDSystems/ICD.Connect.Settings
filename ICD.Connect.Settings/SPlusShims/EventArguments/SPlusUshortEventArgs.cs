using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.SPlusShims.EventArguments
{
	public sealed class SPlusUshortEventArgs : EventArgs
	{
		[PublicAPI("S+")]
		public ushort Data { get; set; }

		[PublicAPI("S+")]
		public SPlusUshortEventArgs(){}

		public SPlusUshortEventArgs(ushort data)
		{
			Data = data;
		}
	}
}