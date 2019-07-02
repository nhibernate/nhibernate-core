using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Shift
	{
		public virtual byte Id { get; set; }
		public virtual DateTime EndTime { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime StartTime { get; set; }
		public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistory { get; set; }
	}
}
