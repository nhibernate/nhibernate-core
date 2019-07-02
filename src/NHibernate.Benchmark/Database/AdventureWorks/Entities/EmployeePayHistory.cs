using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class EmployeePayHistory
	{
		public virtual DateTime RateChangeDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual byte PayFrequency { get; set; }
		public virtual decimal Rate { get; set; }

		public virtual Employee BusinessEntity { get; set; }
	}
}
