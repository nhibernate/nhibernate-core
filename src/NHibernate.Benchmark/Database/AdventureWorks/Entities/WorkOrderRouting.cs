using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class WorkOrderRouting
	{
		public virtual int ProductId { get; set; }
		public virtual short OperationSequence { get; set; }
		public virtual decimal? ActualCost { get; set; }
		public virtual DateTime? ActualEndDate { get; set; }
		public virtual decimal? ActualResourceHrs { get; set; }
		public virtual DateTime? ActualStartDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual decimal PlannedCost { get; set; }
		public virtual DateTime ScheduledEndDate { get; set; }
		public virtual DateTime ScheduledStartDate { get; set; }

		public virtual Location Location { get; set; }
		public virtual WorkOrder WorkOrder { get; set; }
	}
}
