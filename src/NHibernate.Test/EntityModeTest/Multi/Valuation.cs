using System;

namespace NHibernate.Test.EntityModeTest.Multi
{
	public class Valuation
	{
		public virtual long Id { get; set; }

		public virtual Stock Stock { get; set; }

		public virtual DateTime ValuationDate { get; set; }

		public virtual double Value { get; set; }
	}
}