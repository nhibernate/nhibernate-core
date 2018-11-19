namespace NHibernate.Driver
{
	/// <summary>
	/// This provides a driver for Sybase ASE 15 using the ADO.NET 2 driver.
	/// </summary>
	/// <remarks>
	/// You will need the following libraries available to your application:
	/// <ul>
	/// <li>Sybase.AdoNet2.AseClient.dll</li>
	/// <li>sybdrvado20.dll</li>
	/// </ul>
	/// </remarks>
	public class SybaseAseClientDriver : SybaseAseClientDriverBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SybaseAseClientDriver() : base("Sybase.AdoNet2.AseClient")
		{
		}
	}
}
