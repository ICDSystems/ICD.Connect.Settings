using ICD.Common.Properties;
using ICD.Connect.Settings.Simpl;

namespace ICD.Connect.Settings.SPlusShims
{
	public interface ISPlusOriginatorShim
	{
		/// <summary>
		/// Gets the wrapped originator.
		/// </summary>
		ISimplOriginator Originator { get; }

		/// <summary>
		/// The Simpl Windows Location, set by S+
		/// </summary>
		[PublicAPI("S+")]
		string Location { get; set; }
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