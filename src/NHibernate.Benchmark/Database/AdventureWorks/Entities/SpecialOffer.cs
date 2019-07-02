using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SpecialOffer
	{
		public virtual int Id { get; set; }
		public virtual string Category { get; set; }
		public virtual string Description { get; set; }
		public virtual decimal DiscountPct { get; set; }
		public virtual DateTime EndDate { get; set; }
		public virtual int? MaxQty { get; set; }
		public virtual int MinQty { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual DateTime StartDate { get; set; }
		public virtual string Type { get; set; }
		public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct { get; set; } = new HashSet<SpecialOfferProduct>();
	}
}
