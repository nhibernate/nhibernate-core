using System.Collections.Generic;

namespace NHibernate.Test.EntityModeTest.Multi
{
	public class Stock
	{
		private ISet<Valuation> valuations = new HashSet<Valuation>();

		public virtual long Id { get; set; }

		public virtual string TradeSymbol { get; set; }

		public virtual Valuation CurrentValuation { get; set; }

		public virtual ISet<Valuation> Valuations
		{
			get { return valuations; }
			set { valuations = value; }
		}
	}
}