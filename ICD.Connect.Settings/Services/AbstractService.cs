using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Services
{
	public abstract class AbstractService<TService, TSettings> : AbstractOriginator<TSettings>, IService
		where TService : IService
		where TSettings : IServiceSettings, new()
	{
		private readonly ServiceProviderCollection m_Providers;

		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public override string Category { get { return "Service"; } }

		/// <summary>
		/// Gets the providers for this service.
		/// </summary>
		public ServiceProviderCollection Providers { get { return m_Providers; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractService()
		{
			m_Providers = new ServiceProviderCollection();

			ServiceProvider.AddService(typeof(TService), this);
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			ServiceProvider.RemoveService(this);
		}

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Providers.SetChildren(Enumerable.Empty<IServiceProvider>());
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.ProviderSettings.SetRange(CopySerializableSettings(Providers));
		}

		private static IEnumerable<ISettings> CopySerializableSettings<TOriginator>(IEnumerable<TOriginator> collection)
			where TOriginator : class, IOriginator
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			return collection.Where(c => c.Serialize)
			                 .Select(r => r.CopySettings());
		}

		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			IEnumerable<IServiceProvider> connections =
				GetOriginatorsSkipExceptions<IServiceProvider>(settings.ProviderSettings, factory);
			m_Providers.SetChildren(connections);
		}

		private IEnumerable<T> GetOriginatorsSkipExceptions<T>(IEnumerable<ISettings> originatorSettings,
															   IDeviceFactory factory)
			where T : class, IOriginator
		{
			foreach (ISettings settings in originatorSettings)
			{
				T output;

				try
				{
					output = factory.GetOriginatorById<T>(settings.Id);
				}
				catch (Exception e)
				{
					Logger.Log(eSeverity.Error, "Failed to instantiate {0} with id {1} - {2}", typeof(T).Name, settings.Id, e.Message);
					continue;
				}

				yield return output;
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return ConsoleNodeGroup.KeyNodeMap("Providers", "The providers for this service", Providers, p => (uint)p.Id);
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
