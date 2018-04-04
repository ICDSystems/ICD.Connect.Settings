using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Utils.Services;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Info;
using ICD.Connect.API.Nodes;
using ICD.Connect.API.Proxies;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings.Proxies
{
	public abstract class AbstractProxyOriginator : AbstractProxy, IProxyOriginator
	{
		public event EventHandler OnSettingsClearing;
		public event EventHandler OnSettingsCleared;
		public event EventHandler OnSettingsApplied;

		#region Properties

		/// <summary>
		/// Unique ID for the originator.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// The name of the originator.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The name that is used for the originator while in a combine space.
		/// </summary>
		public string CombineName { get; set; }

		/// <summary>
		/// When true this instance is serialized to the system config.
		/// </summary>
		public bool Serialize { get { return false; } set { throw new NotSupportedException(); } }

		/// <summary>
		/// Logger for the originator.
		/// </summary>
		public ILoggerService Logger { get { return ServiceProvider.TryGetService<ILoggerService>(); } }

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public virtual string ConsoleName { get { return string.IsNullOrEmpty(Name) ? GetType().Name : Name; } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return string.Empty; } }

		#endregion

		#region Methods

		/// <summary>
		/// Set of permissions specific to this originator
		/// </summary>
		public IEnumerable<Permission> GetPermissions()
		{
			yield break;
		}

		/// <summary>
		/// Copies the current instance settings.
		/// </summary>
		/// <returns></returns>
		public ISettings CopySettings()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Copies the current instance properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		public void CopySettings(ISettings settings)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Applies the settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		public void ApplySettings(ISettings settings, IDeviceFactory factory)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Resets the originator to a default state.
		/// </summary>
		public void ClearSettings()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnSettingsApplied = null;
			OnSettingsCleared = null;
			OnSettingsClearing = null;

			base.DisposeFinal(disposing);
		}

		#endregion

		/// <summary>
		/// Override to build initialization commands on top of the current class info.
		/// </summary>
		/// <param name="command"></param>
		protected override void Initialize(ApiClassInfo command)
		{
			base.Initialize(command);

			ApiCommandBuilder.UpdateCommand(command)
			                 .GetProperty(OriginatorApi.PROPERTY_NAME)
			                 .GetProperty(OriginatorApi.PROPERTY_COMBINE_NAME)
			                 .Complete();
		}

		#region Console

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			return OriginatorConsole.GetConsoleNodes(this);
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public virtual void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			OriginatorConsole.BuildConsoleStatus(this, addRow);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			return OriginatorConsole.GetConsoleCommands(this);
		}

		#endregion
	}
}
