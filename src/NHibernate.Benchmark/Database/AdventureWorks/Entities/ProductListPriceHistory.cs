using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductListPriceHistory
	{
		public virtual DateTime StartDate { get; set; }
		public virtual DateTime? EndDate { get; set; }
		public virtual decimal ListPrice { get; set; }
		public virtual DateTime ModifiedDate { get; set; }

		public virtual Product Product { get; set; }
	}
}
