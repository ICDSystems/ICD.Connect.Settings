using System;
﻿using ICD.Connect.API.Attributes;

namespace ICD.Connect.Settings.Originators.Simpl
{
	public interface ISimplOriginator : IOriginator
	{
		[ApiEvent(SimplOriginatorApi.EVENT_ON_REQUEST_SHIM_RESYNC, SimplOriginatorApi.EVENT_ON_REQUEST_SHIM_RESYNC_HELP)]
		event EventHandler<RequestShimResyncEventArgs> OnRequestShimResync;
	}
}
