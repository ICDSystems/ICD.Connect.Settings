using System;
using ICD.Connect.API;
using ICD.Connect.API.Info;

namespace ICD.Connect.Settings
{
    public interface IProxyOriginator : IOriginator
	{
		/// <summary>
		/// Raised when the proxy originator makes an API request.
		/// </summary>
		event EventHandler<ApiClassInfoEventArgs> OnCommand;

		/// <summary>
		/// Called to update the proxy originator with an API result.
		/// </summary>
		/// <param name="result"></param>
		void ParseResult(ApiResult result);
	}
}
