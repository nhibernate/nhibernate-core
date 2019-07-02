using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class UnitMeasure
	{
		public virtual string UnitMeasureCode { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<BillOfMaterials> BillOfMaterials { get; set; } = new HashSet<BillOfMaterials>();
		public virtual ICollection<Product> Product { get; set; } = new HashSet<Product>();
		public virtual ICollection<Product> ProductNavigation { get; set; } = new HashSet<Product>();
		public virtual ICollection<ProductVendor> ProductVendor { get; set; } = new HashSet<ProductVendor>();
	}
}
