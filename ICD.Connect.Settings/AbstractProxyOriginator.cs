﻿using System;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API;
using ICD.Connect.API.Info;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Settings
{
	public abstract class AbstractProxyOriginator : AbstractOriginator<NullSettings>, IProxyOriginator
	{
		/// <summary>
		/// Raised when the proxy originator makes an API request.
		/// </summary>
		public event EventHandler<ApiClassInfoEventArgs> OnCommand;

		#region Methods

		/// <summary>
		/// Called to update the proxy originator with an API result.
		/// </summary>
		/// <param name="result"></param>
		public virtual void ParseResult(ApiResult result)
		{
		}

		#endregion

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			OnCommand = null;

			base.DisposeFinal(disposing);
		}

		/// <summary>
		/// Raises the OnCommand event with the given command.
		/// </summary>
		/// <param name="command"></param>
		protected void SendCommand(ApiClassInfo command)
		{
			if (command == null)
				throw new ArgumentNullException();

			OnCommand.Raise(this, new ApiClassInfoEventArgs(command));
		}

		#region Settings

		protected override sealed void ApplySettingsFinal(NullSettings settings, IDeviceFactory factory)
		{
		}

		protected override sealed void ClearSettingsFinal()
		{
		}

		protected override sealed void CopySettingsFinal(NullSettings settings)
		{
		}

		#endregion
	}
}
