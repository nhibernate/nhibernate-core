using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class CountryRegionCurrency
	{
		public virtual string CountryRegionCode { get; set; }
		public virtual string CurrencyCode { get; set; }
		public virtual DateTime ModifiedDate { get; set; }

		public virtual CountryRegion CountryRegionCodeNavigation { get; set; }
		public virtual Currency CurrencyCodeNavigation { get; set; }
	}
}
