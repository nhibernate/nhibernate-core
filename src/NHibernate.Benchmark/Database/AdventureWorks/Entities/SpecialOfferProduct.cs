using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SpecialOfferProduct
	{
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }

		public virtual ICollection<SalesOrderDetail> SalesOrderDetail { get; set; } = new HashSet<SalesOrderDetail>();
		public virtual Product Product { get; set; }
		public virtual SpecialOffer SpecialOffer { get; set; }
	}
}
