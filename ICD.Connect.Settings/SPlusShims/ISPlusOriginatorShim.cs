using ICD.Connect.Settings.Originators.Simpl;

namespace ICD.Connect.Settings.SPlusShims
{
	public interface ISPlusOriginatorShim : ISPlusShim
	{
		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		ISimplOriginator Originator { get; }
	}

	public interface ISPlusOriginatorShim<TOriginator> : ISPlusOriginatorShim
		where TOriginator : ISimplOriginator
	{
		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		new TOriginator Originator { get; }
	}
}
