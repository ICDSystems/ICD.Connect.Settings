using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.CrestronSPlus.SPlusShims.EventArguments
{
	public sealed class SPlusBoolEventArgs : EventArgs
	{
		[PublicAPI("S+")]
		public bool Data { get; set; }

		[PublicAPI("S+")]
		public ushort SPlusData{get { return Data.ToUShort(); }}

		[PublicAPI("S+")]
		public SPlusBoolEventArgs(){}

		public SPlusBoolEventArgs(bool data)
		{
			Data = data;
		}


	}
}