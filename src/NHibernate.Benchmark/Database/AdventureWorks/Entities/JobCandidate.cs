using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class JobCandidate
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Resume { get; set; }

		public virtual Employee BusinessEntity { get; set; }
	}
}
