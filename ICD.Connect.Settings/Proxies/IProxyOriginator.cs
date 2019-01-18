using ICD.Connect.API.Proxies;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Proxies
{
    public interface IProxyOriginator : IOriginator, IProxy
	{
	}

	public interface IProxyOriginator<TSettings> : IProxyOriginator, IOriginator<TSettings>
		where TSettings : IProxySettings
	{
	}
}
