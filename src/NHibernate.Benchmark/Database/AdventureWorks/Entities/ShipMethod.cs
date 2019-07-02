using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ShipMethod
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual decimal ShipBase { get; set; }
		public virtual decimal ShipRate { get; set; }

		public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeader { get; set; } = new HashSet<PurchaseOrderHeader>();
		public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
	}
}
