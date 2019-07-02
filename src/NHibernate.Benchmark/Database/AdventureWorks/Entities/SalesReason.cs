using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesReason
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual string ReasonType { get; set; }

		public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReason { get; set; } = new HashSet<SalesOrderHeaderSalesReason>();
	}
}
