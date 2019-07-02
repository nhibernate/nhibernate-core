using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesPerson
	{
		public virtual decimal Bonus { get; set; }
		public virtual decimal CommissionPct { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid rowguid { get; set; }
		public virtual decimal SalesLastYear { get; set; }
		public virtual decimal? SalesQuota { get; set; }

		public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
		public virtual ICollection<SalesPersonQuotaHistory> SalesPersonQuotaHistory { get; set; } = new HashSet<SalesPersonQuotaHistory>();
		public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistory { get; set; } = new HashSet<SalesTerritoryHistory>();
		public virtual ICollection<Store> Store { get; set; } = new HashSet<Store>();
		public virtual Employee BusinessEntity { get; set; }
		public virtual SalesTerritory Territory { get; set; }
	}
}
