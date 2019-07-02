using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductModelProductDescriptionCulture
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Culture Culture { get; set; }
		public virtual ProductDescription ProductDescription { get; set; }
		public virtual ProductModel ProductModel { get; set; }
	}
}
