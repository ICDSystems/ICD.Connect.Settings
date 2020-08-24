using System;

namespace ICD.Connect.Settings.CrestronSPlus.SPlusShims.GlobalEvents
{
	public sealed class GlobalEventCallback<T> : IGlobalEventCallback<T>
		where T : ISPlusEventInfo
	{
		private readonly Action<T> m_Callback;

		public Action<T> Callback { get { return m_Callback; } }

		object IGlobalEventCallback.Callback { get { return Callback; } }

		public GlobalEventCallback(Action<T> del)
		{
			m_Callback = del;
		}

		void IGlobalEventCallback.Raise(ISPlusEventInfo eventInfo)
		{
			if (!(eventInfo is T))
				throw new ArgumentException("eventInfo");

			Raise((T)eventInfo);
		}

		public void Raise(T eventInfo)
		{
			Callback(eventInfo);
		}
	}
}