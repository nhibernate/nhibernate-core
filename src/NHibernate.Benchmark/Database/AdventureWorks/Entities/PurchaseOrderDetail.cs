using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class PurchaseOrderDetail
	{
		public virtual int Id { get; set; }
		public virtual DateTime DueDate { get; set; }
		public virtual decimal LineTotal { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual short OrderQty { get; set; }
		public virtual decimal ReceivedQty { get; set; }
		public virtual decimal RejectedQty { get; set; }
		public virtual decimal StockedQty { get; set; }
		public virtual decimal UnitPrice { get; set; }
		public virtual Product Product { get; set; }
		public virtual PurchaseOrderHeader PurchaseOrder { get; set; }
	}
}
