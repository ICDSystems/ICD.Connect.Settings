using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.SPlusShims.EventArguments
{
	public sealed class SPlusUShortEventArgs : EventArgs
	{
		[PublicAPI("S+")]
		public ushort Data { get; set; }

		[PublicAPI("S+")]
		public SPlusUShortEventArgs(){}

		public SPlusUShortEventArgs(ushort data)
		{
			Data = data;
		}
	}
}