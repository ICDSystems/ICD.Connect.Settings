using ICD.Connect.API.Proxies;

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
