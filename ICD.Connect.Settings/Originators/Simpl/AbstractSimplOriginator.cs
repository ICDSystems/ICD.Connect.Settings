namespace ICD.Connect.Settings.Originators.Simpl
{
	public abstract class AbstractSimplOriginator<TSettings> : AbstractOriginator<TSettings>, ISimplOriginator
		where TSettings : ISimplOriginatorSettings, new()
	{
	}
}
