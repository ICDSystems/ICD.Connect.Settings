using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Services
{
	public interface IService : IOriginator
	{
		/// <summary>
		/// Gets the providers for this service.
		/// </summary>
		ServiceProviderCollection Providers { get; }
	}
}
