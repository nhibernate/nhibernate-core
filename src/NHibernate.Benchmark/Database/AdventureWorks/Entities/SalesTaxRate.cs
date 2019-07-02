using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class SalesTaxRate
	{
		public virtual int Id { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid RowGuid { get; set; }
		public virtual decimal TaxRate { get; set; }
		public virtual byte TaxType { get; set; }
		public virtual StateProvince StateProvince { get; set; }
	}
}
