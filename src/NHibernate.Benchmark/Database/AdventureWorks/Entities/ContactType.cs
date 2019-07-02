using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class ContactType
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<BusinessEntityContact> BusinessEntityContact { get; set; } = new HashSet<BusinessEntityContact>();
	}
}
