using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class StateProvince
	{
		public virtual int Id { get; set; }
		public virtual string CountryRegionCode { get; set; }
		public virtual bool IsOnlyStateProvinceFlag { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid RowGuid { get; set; }
		public string StateProvinceCode { get; set; }
		public virtual ICollection<Address> Address { get; set; }
		public virtual ICollection<SalesTaxRate> SalesTaxRate { get; set; }
		public virtual CountryRegion CountryRegionCodeNavigation { get; set; }
		public virtual SalesTerritory Territory { get; set; }
	}
}
