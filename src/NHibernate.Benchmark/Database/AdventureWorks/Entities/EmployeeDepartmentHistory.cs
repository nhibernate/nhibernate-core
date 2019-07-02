using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class EmployeeDepartmentHistory
	{
		public virtual DateTime StartDate { get; set; }
		public virtual DateTime? EndDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }

		public virtual Employee BusinessEntity { get; set; }
		public virtual Department Department { get; set; }
		public virtual Shift Shift { get; set; }
	}
}
