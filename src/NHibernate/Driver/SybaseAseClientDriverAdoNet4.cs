namespace NHibernate.Driver
{
	/// <summary>
	/// This provides a driver for Sybase ASE 15 using the ADO.NET 4 driver.
	/// </summary>
	public class SybaseAdoNet4Driver : SybaseAseClientDriverBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SybaseAdoNet4Driver() : base("Sybase.AdoNet4.AseClient")
		{
		}
	}
}
