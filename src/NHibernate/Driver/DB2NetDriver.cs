namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Net5.IBM.Data.Db2/Net.IBM.Data.Db2 DataProvider.
	/// </summary>
	public class DB2NetDriver : DB2DriverBase
	{
		public DB2NetDriver() : base("IBM.Data.Db2")
		{
		}
	}
}
