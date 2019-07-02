using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class WorkOrder
	{
		public virtual int Id { get; set; }
		public virtual DateTime DueDate { get; set; }
		public virtual DateTime? EndDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual int OrderQty { get; set; }
		public virtual short ScrappedQty { get; set; }
		public virtual DateTime StartDate { get; set; }
		public virtual int StockedQty { get; set; }
		public virtual ICollection<WorkOrderRouting> WorkOrderRouting { get; set; } = new HashSet<WorkOrderRouting>();
		public virtual Product Product { get; set; }
		public virtual ScrapReason ScrapReason { get; set; }
	}
}
