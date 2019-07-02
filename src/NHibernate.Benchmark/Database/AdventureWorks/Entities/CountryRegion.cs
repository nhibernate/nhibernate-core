using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class CountryRegion
	{
		public virtual string CountryRegionCode { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }

		public virtual ICollection<CountryRegionCurrency> CountryRegionCurrency { get; set; } = new HashSet<CountryRegionCurrency>();
		public virtual ICollection<SalesTerritory> SalesTerritory { get; set; } = new HashSet<SalesTerritory>();
		public virtual ICollection<StateProvince> StateProvince { get; set; } = new HashSet<StateProvince>();
	}
}
