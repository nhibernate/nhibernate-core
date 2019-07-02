using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Store
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid rowguid { get; set; }
		public virtual string Demographics { get; set; }

		public virtual ICollection<Customer> Customer { get; set; }
		public virtual BusinessEntity BusinessEntity { get; set; }
		public virtual SalesPerson SalesPerson { get; set; }
	}
}
