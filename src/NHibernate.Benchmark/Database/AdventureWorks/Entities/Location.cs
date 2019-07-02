using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Location
	{
		public virtual short Id { get; set; }
		public virtual decimal Availability { get; set; }
		public virtual decimal CostRate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<ProductInventory> ProductInventory { get; set; } = new HashSet<ProductInventory>();
		public virtual ICollection<WorkOrderRouting> WorkOrderRouting { get; set; } = new HashSet<WorkOrderRouting>();
	}
}
