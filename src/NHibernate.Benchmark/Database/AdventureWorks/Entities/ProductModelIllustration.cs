using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductModelIllustration
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Illustration Illustration { get; set; }
		public virtual ProductModel ProductModel { get; set; }
	}
}
