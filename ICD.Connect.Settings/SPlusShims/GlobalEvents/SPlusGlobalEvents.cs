using System;
using System.Collections.Generic;
using ICD.Common.Utils;

namespace ICD.Connect.Settings.SPlusShims.GlobalEvents
{
	public static class SPlusGlobalEvents
	{
		private static readonly Dictionary<Type, List<IGlobalEventCallback>> s_Delegates =
			new Dictionary<Type, List<IGlobalEventCallback>>();

		private static readonly SafeCriticalSection s_DelegatesSafeCriticalSection = 
			new SafeCriticalSection();

		public static void RaiseEvent(ISPlusEventInfo eventInfo)
		{
			Type infoType = eventInfo.GetType();

			s_DelegatesSafeCriticalSection.Enter();
			try
			{
				if (!s_Delegates.ContainsKey(infoType))
					return;

				foreach (var del in s_Delegates[infoType])
					del.Raise(eventInfo);
			}
			finally
			{
				s_DelegatesSafeCriticalSection.Leave();
			}
		}

		public static void RegisterCallback<T>(Action<T> del)
			where T : ISPlusEventInfo
		{
			s_DelegatesSafeCriticalSection.Enter();
			try
			{
				if (!s_Delegates.ContainsKey(typeof(T)))
				{
					s_Delegates.Add(typeof(T), new List<IGlobalEventCallback>());
				}

				s_Delegates[typeof(T)].Add(new GlobalEventCallback<T>(del));
			}
			finally
			{
				s_DelegatesSafeCriticalSection.Leave();
			}
		}

		public static void UnregisterCallback<T>(Action<T> del)
			where T : ISPlusEventInfo
		{
			s_DelegatesSafeCriticalSection.Enter();
			try
			{
				if (!s_Delegates.ContainsKey(typeof(T)))
					return;

				s_Delegates[typeof(T)].RemoveAll(callback => ReferenceEquals(callback.Callback, del));
			}
			finally
			{
				s_DelegatesSafeCriticalSection.Leave();
			}
		}

		public static void ClearCallbacksOfType<T>()
		{
			s_DelegatesSafeCriticalSection.Enter();
			try
			{
				if (!s_Delegates.ContainsKey(typeof(T)))
					return;

				s_Delegates[typeof(T)].Clear();
			}
			finally
			{
				s_DelegatesSafeCriticalSection.Leave();
			}
		}

		public static void ClearAllCallbacks()
		{
			s_DelegatesSafeCriticalSection.Enter();
			try
			{
				s_Delegates.Clear();
			}
			finally
			{
				s_DelegatesSafeCriticalSection.Leave();
			}
		}
	}
}