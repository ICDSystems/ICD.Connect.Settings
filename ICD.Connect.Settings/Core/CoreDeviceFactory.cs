using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;

namespace ICD.Connect.Settings.Core
{
	/// <summary>
	/// CoreDeviceFactory wraps an ICoreSettings to provide a device factory.
	/// </summary>
	public sealed class CoreDeviceFactory : IDeviceFactory
	{
		public event OriginatorLoadedCallback OnOriginatorLoaded;

		private readonly Dictionary<int, IOriginator> m_OriginatorCache;
		private readonly ICoreSettings m_CoreSettings;

		/// <summary>
		/// Keep track of originators instantiated as dependencies so we can catch cyclic dependencies.
		/// </summary>
		private readonly Stack<int> m_Dependencies; 

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="coreSettings"></param>
		public CoreDeviceFactory(ICoreSettings coreSettings)
		{
			m_OriginatorCache = new Dictionary<int, IOriginator>();
			m_Dependencies = new Stack<int>();
			m_CoreSettings = coreSettings;
		}

		#region Methods

		/// <summary>
		/// Returns true if the factory contains any settings that will resolve to the given originator type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool HasOriginators<T>()
			where T : class, IOriginator
		{
			return m_CoreSettings.OriginatorSettings.Any(s => s.OriginatorType.IsAssignableTo(typeof(T)));
		}

		public IEnumerable<T> GetOriginators<T>()
			where T : class, IOriginator
		{
			return m_CoreSettings.OriginatorSettings
			                     .Where(s => s.OriginatorType.IsAssignableTo(typeof(T)))
			                     .Select(s => GetOriginatorById<T>(s.Id));
		}

		[NotNull]
		public T GetOriginatorById<T>(int id)
			where T : class, IOriginator
		{
			return LazyLoadOriginator<T>(id);
		}

		[NotNull]
		public IOriginator GetOriginatorById(int id)
		{
			return LazyLoadOriginator<IOriginator>(id);
		}

		public IEnumerable<int> GetOriginatorIds()
		{
			return m_CoreSettings.OriginatorSettings.Select(s => s.Id);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the originator with the given id.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		[NotNull]
		private T LazyLoadOriginator<T>(int id)
			where T : class, IOriginator
		{
			if (!m_OriginatorCache.ContainsKey(id))
			{
				m_OriginatorCache[id] = InstantiateOriginatorWithId<T>(id);

				OriginatorLoadedCallback handler = OnOriginatorLoaded;
				if (handler != null)
					handler(m_OriginatorCache[id]);
			}

			return (T)m_OriginatorCache[id];
		}

		/// <summary>
		/// Builds the originator from the settings collection with the given id.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		[NotNull]
		private T InstantiateOriginatorWithId<T>(int id)
			where T : IOriginator
		{
			if (m_OriginatorCache.ContainsKey(id))
				throw new InvalidOperationException(string.Format("An originator with id {0} has already been instantiated", id));

			if (!m_CoreSettings.OriginatorSettings.ContainsId(id))
				throw new KeyNotFoundException(string.Format("No settings with id {0}", id));

			ISettings settings = m_CoreSettings.OriginatorSettings.GetById(id);

			Type originatorType = settings.OriginatorType;
			if (!originatorType.IsAssignableTo(typeof(T)))
				throw new InvalidOperationException(string.Format("{0} will not yield an originator of type {1}",
				                                                  settings.GetType().Name, typeof(T).Name));

			T output;

			PushDependency(id);

			try
			{
				// Instantiate the originator
				output = (T)ReflectionUtils.CreateInstance(originatorType);

				// This instance came from settings, so we want to store it back to settings.
				output.Serialize = true;
				output.ApplySettings(settings, this);
			}
			finally
			{
				PopDependency(id);
			}

			return output;
		}

		/// <summary>
		/// Checks for cyclic dependencies.
		/// </summary>
		/// <param name="id"></param>
		private void PushDependency(int id)
		{
			if (m_Dependencies.Contains(id))
				throw new InvalidOperationException("Cyclic dependency detected");

			m_Dependencies.Push(id);
		}

		/// <summary>
		/// Pops the current dependency.
		/// </summary>
		/// <param name="id"></param>
		private void PopDependency(int id)
		{
			if (m_Dependencies.Peek() != id)
				throw new InvalidOperationException("Unexpected dependency state");

			m_Dependencies.Pop();
		}

		#endregion
	}
}
