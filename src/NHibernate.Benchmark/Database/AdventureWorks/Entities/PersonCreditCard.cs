using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class PersonCreditCard
	{
		public virtual DateTime ModifiedDate { get; set; }

		public virtual Person BusinessEntity { get; set; }
		public virtual CreditCard CreditCard { get; set; }
	}
}
