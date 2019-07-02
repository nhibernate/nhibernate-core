using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductProductPhoto
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual bool Primary { get; set; }
		public virtual Product Product { get; set; }
		public virtual ProductPhoto ProductPhoto { get; set; }
	}
}
