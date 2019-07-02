using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Vendor
	{
		public virtual string AccountNumber { get; set; }
		public virtual bool ActiveFlag { get; set; }
		public virtual byte CreditRating { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual bool PreferredVendorStatus { get; set; }
		public virtual string PurchasingWebServiceURL { get; set; }

		public virtual ICollection<ProductVendor> ProductVendor { get; set; } = new HashSet<ProductVendor>();
		public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeader { get; set; } = new HashSet<PurchaseOrderHeader>();
		public virtual BusinessEntity BusinessEntity { get; set; }
	}
}
