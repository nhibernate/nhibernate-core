using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductInventory
	{
		public virtual byte Bin { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual short Quantity { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual string Shelf { get; set; }

		public virtual Location Location { get; set; }
		public virtual Product Product { get; set; }
	}
}
