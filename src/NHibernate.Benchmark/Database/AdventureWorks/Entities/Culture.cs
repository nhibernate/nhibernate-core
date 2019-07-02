using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Culture
	{
		public virtual string CultureID { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture { get; set;} = new HashSet<ProductModelProductDescriptionCulture>();
	}
}
