using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductCategory
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual ICollection<ProductSubcategory> ProductSubcategory { get; set; } = new HashSet<ProductSubcategory>();
	}
}
