namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2.Core DataProvider.
	/// </summary>
	public class DB2CoreDriver : DB2DriverBase
	{		
		public DB2CoreDriver() : base("IBM.Data.DB2.Core")
		{
		}
	}
}
