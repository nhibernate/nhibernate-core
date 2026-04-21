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
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(DB2CoreDriver));
		
		public DB2CoreDriver() : base("IBM.Data.DB2.Core")
		{
		    Log.Warn("DB2CoreDriver is obsolete, please use DB2NetDriver instead.");
		}
	}
}
