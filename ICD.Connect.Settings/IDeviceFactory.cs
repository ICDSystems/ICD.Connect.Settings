using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings
{
	public delegate void OriginatorLoadedCallback(IOriginator originator);

	/// <summary>
	/// IDeviceFactory represents a factory that instantiates dependencies.
	/// 
	/// Since devices have a dependency on ports, and ports are often dependent on
	/// existing devices, the IDeviceFactory is passed into instantiation methods
	/// to allow "daisy-chaining".
	/// </summary>
	public interface IDeviceFactory
	{
		/// <summary>
		/// Raised each time an originator is initially loaded
		/// </summary>
		event OriginatorLoadedCallback OnOriginatorLoaded;

		/// <summary>
		/// Returns true if the factory contains any settings that will resolve to the given originator type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[PublicAPI]
		bool HasOriginators<T>() where T : class, IOriginator;

		/// <summary>
		/// Lazy-loads the originator with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[PublicAPI]
		IOriginator GetOriginatorById(int id);

		/// <summary>
		/// Gets all of the available ids for originators that can be instantiated.
		/// </summary>
		/// <returns></returns>
		[PublicAPI]
		IEnumerable<int> GetOriginatorIds();

		/// <summary>
		/// Lazy loads the originators of the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[PublicAPI]
		IEnumerable<T> GetOriginators<T>() where T : class, IOriginator;

		/// <summary>
		/// Lazy loads the originator with the given id and casts to the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		[PublicAPI]
		T GetOriginatorById<T>(int id) where T : class, IOriginator;
	}

	public static class DeviceFactoryExtensions
	{
		/// <summary>
		/// Lazy-loads and returns all of the originators.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IOriginator> GetOriginators(this IDeviceFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			return factory.GetOriginators(factory.GetOriginatorIds());
		}

		/// <summary>
		/// Lazy-loads and returns the originators with the given ids.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="ids"></param>
		/// <returns></returns>
		[PublicAPI]
		public static IEnumerable<IOriginator> GetOriginators(this IDeviceFactory factory, IEnumerable<int> ids)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			if (ids == null)
				throw new ArgumentNullException("ids");

			return ids.Select(o => factory.GetOriginatorById(o));
		}

		/// <summary>
		/// Lazy-loads the originator with the given id.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="id"></param>
		[PublicAPI]
		public static void LoadOriginator(this IDeviceFactory factory, int id)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			factory.GetOriginatorById(id);
		}

		/// <summary>
		/// Lazy-loads all of the originators.
		/// </summary>
		/// <param name="factory"></param>
		[PublicAPI]
		public static void LoadOriginators(this IDeviceFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			factory.GetOriginators().Execute();
		}

		/// <summary>
		/// Lazy-loads the originators for the given ids.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="ids"></param>
		[PublicAPI]
		public static void LoadOriginators(this IDeviceFactory factory, IEnumerable<int> ids)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			if (ids == null)
				throw new ArgumentNullException("ids");

			factory.GetOriginators(ids).Execute();
		}

		/// <summary>
		/// Lazy-loads the originators of the given type. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="factory"></param>
		[PublicAPI]
		public static void LoadOriginators<T>(this IDeviceFactory factory)
			where T : class, IOriginator
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			factory.GetOriginators<T>().Execute();
		}
	}
}
