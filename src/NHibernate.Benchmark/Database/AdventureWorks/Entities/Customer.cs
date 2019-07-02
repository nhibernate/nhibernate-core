using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Customer
	{
		public virtual int Id { get; set; }
		public virtual string AccountNumber { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public Guid RowGuid { get; set; }

		public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
		public virtual Person Person { get; set; }
		public virtual Store Store { get; set; }
		public virtual SalesTerritory Territory { get; set; }
	}
}
