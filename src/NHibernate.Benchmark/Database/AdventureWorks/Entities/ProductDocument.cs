using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ProductDocument
	{
		public virtual int Id { get; set; }
		public virtual int DocumentNode { get; set; }
		public virtual DateTime ModifiedDate { get; set; }

		public virtual Product Product { get; set; }
	}
}
