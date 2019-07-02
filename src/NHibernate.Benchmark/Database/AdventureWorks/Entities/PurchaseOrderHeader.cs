using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class PurchaseOrderHeader
	{
		public virtual int Id { get; set; }
		public virtual decimal Freight { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual DateTime OrderDate { get; set; }
		public virtual byte RevisionNumber { get; set; }
		public virtual DateTime? ShipDate { get; set; }
		public virtual byte Status { get; set; }
		public virtual decimal SubTotal { get; set; }
		public virtual decimal TaxAmt { get; set; }
		public virtual decimal TotalDue { get; set; }
		public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetail { get; set; } = new HashSet<PurchaseOrderDetail>();
		public virtual Employee Employee { get; set; }
		public virtual ShipMethod ShipMethod { get; set; }
		public virtual Vendor Vendor { get; set; }
	}
}
