using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Services
{
	public abstract class AbstractServiceProvider<TSettings> : AbstractOriginator<TSettings>, IServiceProvider
		where TSettings : IServiceProviderSettings, new()
	{
		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public override string Category { get { return "Service Provider"; } }
	}
}
