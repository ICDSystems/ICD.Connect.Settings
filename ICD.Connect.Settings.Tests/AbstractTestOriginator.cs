using System;
using System.Collections.Generic;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Permissions;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Settings.Cores;
using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.Tests
{
	public abstract class AbstractTestOriginator<TSettings> : IOriginator<TSettings>
		where TSettings : ISettings
	{
		public event EventHandler OnSettingsClearing;
		public event EventHandler OnSettingsCleared;
		public event EventHandler OnSettingsApplied;
		public event EventHandler OnNameChanged;
		public event EventHandler<BoolEventArgs> OnDisableStateChanged;

		private ICore m_CachedCore;

		/// <summary>
		/// Gets the parent core instance.
		/// </summary>
		public ICore Core { get { return m_CachedCore = m_CachedCore ?? ServiceProvider.GetService<ICore>(); } }

		/// <summary>
		/// Gets the category for this originator type (e.g. Device, Port, etc)
		/// </summary>
		public abstract string Category { get; }

		public int Id { get; set; }

		public Guid Uuid { get; set; }

		public string Name { get; set; }

		public string CombineName { get; set; }

		/// <summary>
		/// Human readable text describing the originator.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Controls the visibility of the originator to the end user.
		/// Useful for hiding logical switchers, duplicate sources, etc.
		/// </summary>
		public bool Hide { get; set; }

		public bool Disable { get; set; }

		public int Order { get; set; }

		public IEnumerable<Permission> GetPermissions()
		{
			throw new NotImplementedException();
		}

		public bool Serialize { get; set; }

		public ILoggingContext Logger { get; set; }

		public ISettings CopySettings()
		{
			throw new NotImplementedException();
		}

		public void CopySettings(TSettings settings)
		{
			throw new NotImplementedException();
		}

		public void ApplySettings(TSettings settings, IDeviceFactory factory)
		{
			throw new NotImplementedException();
		}

		TSettings IOriginator<TSettings>.CopySettings()
		{
			throw new NotImplementedException();
		}

		public void CopySettings(ISettings settings)
		{
			throw new NotImplementedException();
		}

		public void ApplySettings(ISettings settings, IDeviceFactory factory)
		{
			throw new NotImplementedException();
		}

		public void ClearSettings()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public string ConsoleName { get { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public string ConsoleHelp { get { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets the child console nodes.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			throw new NotImplementedException();
		}
	}
}
