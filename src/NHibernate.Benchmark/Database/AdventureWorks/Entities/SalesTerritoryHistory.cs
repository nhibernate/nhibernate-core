using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesTerritoryHistory
	{
		public virtual DateTime StartDate { get; set; }
		public virtual DateTime? EndDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual SalesPerson BusinessEntity { get; set; }
		public virtual SalesTerritory Territory { get; set; }
	}
}
