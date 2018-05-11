namespace ICD.Connect.Settings
{
	public interface IUsernamePasswordSettings : ISettings
	{
		/// <summary>
		/// Gets/sets the configurable username.
		/// </summary>
		string Username { get; set; }

		/// <summary>
		/// Gets/sets the configurable password.
		/// </summary>
		string Password { get; set; }
	}
}
