namespace NHibernate.Driver
{
	/// <summary>
	/// This provides a driver for Sybase ASE 16 using the ADO.NET 4.5 driver.
	/// </summary>
	public class SybaseAdoNet45Driver : SybaseAseClientDriverBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SybaseAdoNet45Driver() : base("Sybase.AdoNet45.AseClient")
		{
		}
	}
}
