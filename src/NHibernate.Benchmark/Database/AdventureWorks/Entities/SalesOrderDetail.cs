using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesOrderDetail
	{
		public virtual int Id { get; set; }
		public virtual string CarrierTrackingNumber { get; set; }
		public virtual decimal LineTotal { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual short OrderQty { get; set; }
		public virtual int ProductId { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual int SpecialOfferId { get; set; }
		public virtual decimal UnitPrice { get; set; }
		public virtual decimal UnitPriceDiscount { get; set; }
		public virtual SalesOrderHeader SalesOrder { get; set; }
		public virtual SpecialOfferProduct SpecialOfferProduct { get; set; }
	}
}
