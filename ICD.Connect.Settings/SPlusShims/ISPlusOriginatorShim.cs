using ICD.Connect.Settings.Originators;

namespace ICD.Connect.Settings.SPlusShims
{
	public interface ISPlusOriginatorShim : ISPlusShim
	{
		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		IOriginator Originator { get; }
	}

	public interface ISPlusOriginatorShim<TOriginator> : ISPlusOriginatorShim
		where TOriginator : IOriginator
	{
		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		new TOriginator Originator { get; }
	}
}
