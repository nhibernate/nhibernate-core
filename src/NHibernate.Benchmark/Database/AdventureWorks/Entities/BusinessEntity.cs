using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class BusinessEntity
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual ICollection<BusinessEntityAddress> BusinessEntityAddress { get; set; } = new HashSet<BusinessEntityAddress>();
		public virtual ICollection<BusinessEntityContact> BusinessEntityContact { get; set; } = new HashSet<BusinessEntityContact>();
		public virtual Person Person { get; set; }
		public virtual Store Store { get; set; }
		public virtual Vendor Vendor { get; set; }
	}
}
