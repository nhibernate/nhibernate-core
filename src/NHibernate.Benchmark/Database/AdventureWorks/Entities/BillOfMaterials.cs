using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
	public class BillOfMaterials
	{
		public virtual int Id { get; set; }
		public virtual short BOMLevel { get; set; }
		public virtual DateTime? EndDate { get; set; }
		public virtual DateTime ModifiedDate { get; set; }
		public virtual decimal PerAssemblyQty { get; set; }
		public virtual DateTime StartDate { get; set; }
		public virtual string UnitMeasureCode { get; set; }

		public virtual Product Component { get; set; }
		public virtual Product ProductAssembly { get; set; }
		public virtual UnitMeasure UnitMeasureCodeNavigation { get; set; }
	}
}
