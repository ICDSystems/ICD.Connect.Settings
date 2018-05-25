namespace ICD.Connect.Settings.Simpl
{
	public abstract class AbstractSimplOriginator<TSettings> : AbstractOriginator<TSettings>, ISimplOriginator
		where TSettings : ISimplOriginatorSettings, new()
	{
	}
}
