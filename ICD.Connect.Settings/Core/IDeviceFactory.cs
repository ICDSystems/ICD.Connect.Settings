using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;

namespace ICD.Connect.Settings.Core
{
	/// <summary>
	/// IDeviceFactory represents a factory that instantiates dependencies.
	/// 
	/// Since devices have a dependency on ports, and ports are often dependent on
	/// existing devices, the IDeviceFactory is passed into instantiation methods
	/// to allow "daisy-chaining".
	/// </summary>
	public interface IDeviceFactory
	{
		[PublicAPI]
		IOriginator GetOriginatorById(int id);

		[PublicAPI]
		IEnumerable<int> GetOriginatorIds();

		[PublicAPI]
		T GetOriginatorById<T>(int id) where T : class, IOriginator;

		//ICore GetCore();
	}

	public static class DeviceFactoryExtensions
	{
		public static IEnumerable<IOriginator> GetOriginators(this IDeviceFactory factory)
		{
			return factory.GetOriginatorIds().Select(o => factory.GetOriginatorById(o));
		}
	}
}
