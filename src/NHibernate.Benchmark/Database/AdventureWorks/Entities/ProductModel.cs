using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductModel
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual string CatalogDescription { get; set; }
		public virtual string Instructions { get; set; }

		public virtual ICollection<Product> Product { get; set; } = new HashSet<Product>();
		public virtual ICollection<ProductModelIllustration> ProductModelIllustration { get; set; } = new HashSet<ProductModelIllustration>();
		public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture { get; set; } = new HashSet<ProductModelProductDescriptionCulture>();
	}
}
