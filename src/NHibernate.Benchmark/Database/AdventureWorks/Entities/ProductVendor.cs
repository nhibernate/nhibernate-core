using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductVendor
	{
		public virtual int AverageLeadTime { get; set; }
		public virtual decimal? LastReceiptCost { get; set; }
		public virtual DateTime? LastReceiptDate { get; set; }
		public virtual int MaxOrderQty { get; set; }
		public virtual int MinOrderQty { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual int? OnOrderQty { get; set; }
		public virtual decimal StandardPrice { get; set; }
		public string UnitMeasureCode { get; set; }
		public virtual Vendor BusinessEntity { get; set; }
		public virtual Product Product { get; set; }
		public virtual UnitMeasure UnitMeasureCodeNavigation { get; set; }
	}
}
