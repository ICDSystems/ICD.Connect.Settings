using System;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.SPlusShims.EventArguments
{
	public sealed class SPlusIntEventArgs : EventArgs
	{
		[PublicAPI("S+")]
		public int Data { get; set; }

		[PublicAPI("S+")]
		public SPlusIntEventArgs(){}

		public SPlusIntEventArgs(int data)
		{
			Data = data;
		}
	}
}