namespace SampleApp.Extensions.Configuration
{
	/// <summary>
	/// This is an example on how you can create configuration component that you can configure in the castle windsors config.
	/// </summary>
	/// <remarks>Properties are set using the properties tag under the configured component using naming convention for either public properties or constructor arguments</remarks>
	public class TabConfiguration
	{
		public bool ShowTab { get; set; }

		public bool ShowUCommerceVersion { get; set; }
		
		public bool ShowShemaVersion { get; set; }

	}
}
