using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductDescription
	{
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture { get; set; } = new HashSet<ProductModelProductDescriptionCulture>();
	}
}
