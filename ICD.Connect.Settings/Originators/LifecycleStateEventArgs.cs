using ICD.Common.Utils.EventArguments;

namespace ICD.Connect.Settings.Originators
{
	/// <summary>
	/// Describes the lifecycle state of the originator
	/// </summary>
	public enum eLifecycleState
	{
		Instantiated, // Originator is instantiated, but not loaded yet
		Loading, // Originator is in process of loading it's settings
		Loaded, // Originator has had it's setting loaded
		Starting, // Originator is int he process of starting settings
		Started,  // Originator has had it's settings started
		Clearing, // Originator is in the process of clearing it's settings
		Cleared, // Originator has had it's settings cleared
		Disposed // Originator has been disposed
	}

	/// <summary>
	/// EventArgs for Lifecycle State Changes
	/// </summary>
	public sealed class LifecycleStateEventArgs : GenericEventArgs<eLifecycleState>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public LifecycleStateEventArgs(eLifecycleState data) : base(data)
		{
		}
	}
}