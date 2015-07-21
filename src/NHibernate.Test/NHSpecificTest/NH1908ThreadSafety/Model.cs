using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1908ThreadSafety
{
	public class Order
	{
		public virtual long Id { get; set; }
		public virtual IList<OrderLine> ActiveOrderLines { get; set;}
		public virtual string Email { get; set; }
	}

	public class OrderLine
	{
		public virtual long Id { get; set; }
		public virtual DateTime ValidUntil { get; set; }
	}
}
