using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Address
	{
		public virtual int Id { get; set; }
		public virtual string AddressLine1 { get; set; }
		public virtual string AddressLine2 { get; set; }
		public virtual string City { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string PostalCode { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual ICollection<BusinessEntityAddress> BusinessEntityAddress { get; set; } = new HashSet<BusinessEntityAddress>();
		public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
		public virtual ICollection<SalesOrderHeader> SalesOrderHeaderNavigation { get; set; } = new HashSet<SalesOrderHeader>();

		public virtual StateProvince StateProvince { get; set; }
	}
}
