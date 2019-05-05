using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2892
{
	public class Order
	{
		public virtual int Id { get; set; }
		public virtual ISet<OrderLine> OrderLines { get; set; } = new HashSet<OrderLine>();
	}
}
