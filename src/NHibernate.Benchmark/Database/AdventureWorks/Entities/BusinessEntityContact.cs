using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class BusinessEntityContact
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public Guid RowGuid { get; set; }

		public virtual BusinessEntity BusinessEntity { get; set; }
		public virtual ContactType ContactType { get; set; }
		public virtual Person Person { get; set; }
	}
}
