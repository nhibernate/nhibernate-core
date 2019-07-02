using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class Currency
	{
		public virtual string CurrencyCode { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<CountryRegionCurrency> CountryRegionCurrency { get; set; } = new HashSet<CountryRegionCurrency>();
		public virtual ICollection<CurrencyRate> CurrencyRate { get; set; } = new HashSet<CurrencyRate>();
		public virtual ICollection<CurrencyRate> CurrencyRateNavigation { get; set; } = new HashSet<CurrencyRate>();
	}
}
