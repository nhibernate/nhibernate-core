using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Illustration
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Diagram { get; set; }

		public virtual ICollection<ProductModelIllustration> ProductModelIllustration { get; set; } = new HashSet<ProductModelIllustration>();
	}
}
