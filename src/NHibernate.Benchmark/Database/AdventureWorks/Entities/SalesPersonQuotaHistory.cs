using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesPersonQuotaHistory
	{
		public virtual DateTime QuotaDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual decimal SalesQuota { get; set; }
		public virtual SalesPerson BusinessEntity { get; set; }
	}
}
