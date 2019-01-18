using ICD.Connect.API.EventArguments;
using ICD.Connect.API.Info;

namespace ICD.Connect.Settings.Originators.Simpl
{
	public sealed class RequestShimResyncEventArgs : AbstractApiEventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public RequestShimResyncEventArgs() : base(SimplOriginatorApi.EVENT_ON_REQUEST_SHIM_RESYNC)
		{
		}

		/// <summary>
		/// Builds an API result for the args.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override void BuildResult(object sender, ApiResult result)
		{
		}
	}
}