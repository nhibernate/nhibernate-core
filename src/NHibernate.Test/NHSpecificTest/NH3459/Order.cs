using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3459
{
	public class Order
	{
		private readonly IEnumerable<OrderLine> _orderLines = new List<OrderLine>();
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual IEnumerable<OrderLine> OrderLines
		{
			get { return _orderLines; }
		}
	}

	public class OrderLine
	{
		public virtual Guid Id { get; set; }
		public virtual string Manufacturer { get; set; }
		public virtual Order Order { get; set; }
	}
}