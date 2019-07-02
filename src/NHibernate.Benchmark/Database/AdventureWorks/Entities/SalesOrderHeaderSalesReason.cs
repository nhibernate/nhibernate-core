using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesOrderHeaderSalesReason
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual SalesOrderHeader SalesOrder { get; set; }
		public virtual SalesReason SalesReason { get; set; }
	}
}
