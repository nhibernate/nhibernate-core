using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Department
	{
		public virtual short Id { get; set; }
		public virtual string GroupName { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistory { get; set; } =
			new HashSet<EmployeeDepartmentHistory>();
	}
}
