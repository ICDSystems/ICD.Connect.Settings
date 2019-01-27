using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Permissions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Cores
{
	public sealed class Core : AbstractCore<CoreSettings>
	{
		/// <summary>
		/// Originator ids are pushed to the stack on load, and popped on clear.
		/// </summary>
		private readonly Stack<int> m_LoadedOriginators;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Core()
		{
			ServiceProvider.AddService<ICore>(this);

			m_LoadedOriginators = new Stack<int>();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			DisposeOriginators();
		}

		/// <summary>
		/// Adds the given originator to the cor.
		/// </summary>
		/// <param name="originator"></param>
		private void AddOriginator(IOriginator originator)
		{
			Originators.AddChild(originator);
		}

		/// <summary>
		/// Unsubscribes, disposes and clears the devices.
		/// </summary>
		private void DisposeOriginators()
		{
			// First try to dispose in reverse of load order
			while (m_LoadedOriginators.Count > 0)
			{
				int id = m_LoadedOriginators.Pop();

				IOriginator originator;
				if (!Originators.TryGetChild(id, out originator))
					continue;

				TryDisposeOriginator(originator);
			}

			// Now dispose the remainder
			foreach (IOriginator originator in Originators)
				TryDisposeOriginator(originator);

			Originators.Clear();
		}

		/// <summary>
		/// Attempts to dispose the originator if it implements IDisposable. Logs any exceptions.
		/// </summary>
		/// <param name="originator"></param>
		private void TryDisposeOriginator(IOriginator originator)
		{
			IDisposable disposable = originator as IDisposable;
			if (disposable == null)
				return;

			try
			{
				disposable.Dispose();
			}
			catch (Exception e)
			{
				Log(eSeverity.Error, "Failed to dispose {0} - {1}", originator, e.Message);
			}
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(CoreSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.OriginatorSettings.Clear();
			settings.OriginatorSettings.AddRange(GetSerializableOriginators());
		}

		private IEnumerable<ISettings> GetSerializableOriginators()
		{
			return Originators.GetChildren()
								.Where(c => c.Serialize)
								.Select(p => p.CopySettings());
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			DisposeOriginators();

			ResetDefaultPermissions();
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(CoreSettings settings, IDeviceFactory factory)
		{
			m_LoadedOriginators.Clear();
			factory.OnOriginatorLoaded += FactoryOnOriginatorLoaded;

			try
			{
				base.ApplySettingsFinal(settings, factory);

				LoadOriginatorsSkipExceptions(factory);

				ResetDefaultPermissions();
			}
			finally
			{
				factory.OnOriginatorLoaded -= FactoryOnOriginatorLoaded;
			}
		}

		private void LoadOriginatorsSkipExceptions(IDeviceFactory factory)
		{
			foreach (int id in factory.GetOriginatorIds())
			{
				try
				{
					// Don't care about the result, loaded originators will be handled by the load event.
					factory.GetOriginatorById(id);
				}
				catch (Exception e)
				{
					Log(eSeverity.Error, "Failed to instantiate {0} with id {1} - {2}", typeof(IOriginator).Name, id, e.Message);
				}
			}
		}

		/// <summary>
		/// Called each time an originator is loaded while applying settings.
		/// </summary>
		/// <param name="originator"></param>
		private void FactoryOnOriginatorLoaded(IDeviceFactory factory, IOriginator originator)
		{
			m_LoadedOriginators.Push(originator.Id);
			AddOriginator(originator);
		}

		private void ResetDefaultPermissions()
		{
			PermissionsManager permissionsManager = ServiceProvider.TryGetService<PermissionsManager>();
			if (permissionsManager != null)
				permissionsManager.SetDefaultPermissions(GetPermissions());
		}

		#endregion
	}
}
