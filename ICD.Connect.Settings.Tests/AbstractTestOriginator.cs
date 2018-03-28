using System;
using System.Collections.Generic;
using ICD.Common.Permissions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings.Tests
{
	public abstract class AbstractTestOriginator<TSettings> : IOriginator<TSettings>
		where TSettings : ISettings
	{
		public event EventHandler OnSettingsClearing;
		public event EventHandler OnSettingsCleared;
		public event EventHandler OnSettingsApplied;

		public int Id { get; set; }

		public string Name { get; set; }

		public string CombineName { get; set; }

		public IEnumerable<Permission> GetPermissions()
		{
			throw new NotImplementedException();
		}

		public bool Serialize { get; set; }

		public ILoggerService Logger { get; set; }

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
	}
}
