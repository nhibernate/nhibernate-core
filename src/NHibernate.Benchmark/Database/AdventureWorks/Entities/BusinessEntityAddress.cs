using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class BusinessEntityAddress
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual Address Address { get; set; }
		public virtual AddressType AddressType { get; set; }
		public virtual BusinessEntity BusinessEntity { get; set; }
	}
}
