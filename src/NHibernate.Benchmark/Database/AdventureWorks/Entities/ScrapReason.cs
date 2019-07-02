using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ScrapReason
	{
		public virtual short Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<WorkOrder> WorkOrder { get; set; } = new HashSet<WorkOrder>();
	}
}
