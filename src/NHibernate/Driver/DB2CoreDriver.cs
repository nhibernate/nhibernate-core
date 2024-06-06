using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2.Core DataProvider.
	/// </summary>
	// Since v5.6
	[Obsolete("Please use DB2NetDriver")]
	public class DB2CoreDriver : DB2NetDriver
	{
		public DB2CoreDriver() : base("IBM.Data.DB2.Core")
		{
		}
	}
}
