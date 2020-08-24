using System;

namespace ICD.Connect.Settings.CrestronSPlus.SPlusShims.GlobalEvents
{
	public interface IGlobalEventCallback
	{
		object Callback { get; }

		void Raise(ISPlusEventInfo eventInfo);
	}

	public interface IGlobalEventCallback<T> : IGlobalEventCallback
		where T : ISPlusEventInfo
	{
		new Action<T> Callback { get; }

		void Raise(T eventInfo);
	}
}