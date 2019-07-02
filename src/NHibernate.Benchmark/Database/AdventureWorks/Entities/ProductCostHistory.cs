using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductCostHistory
	{
		public virtual int Id { get; set; }
		public virtual DateTime StartDate { get; set; }
		public virtual DateTime? EndDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual decimal StandardCost { get; set; }

		public virtual Product Product { get; set; }
	}
}
