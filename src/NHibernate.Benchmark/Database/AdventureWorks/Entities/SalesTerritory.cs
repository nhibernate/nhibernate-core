using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesTerritory
	{
		public virtual int Id { get; set; }
		public virtual decimal CostLastYear { get; set; }
		public virtual decimal CostYTD { get; set; }
		public virtual string CountryRegionCode { get; set; }
		public virtual string Group { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual decimal SalesLastYear { get; set; }
		public virtual decimal SalesYTD { get; set; }

		public virtual ICollection<Customer> Customer { get; set; } = new HashSet<Customer>();
		public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
		public virtual ICollection<SalesPerson> SalesPerson { get; set; } = new HashSet<SalesPerson>();
		public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistory { get; set; } = new HashSet<SalesTerritoryHistory>();
		public virtual ICollection<StateProvince> StateProvince { get; set; } = new HashSet<StateProvince>();
		public virtual CountryRegion CountryRegionCodeNavigation { get; set; }
	}
}
