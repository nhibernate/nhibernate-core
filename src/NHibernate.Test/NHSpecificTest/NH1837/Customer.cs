using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1837
{
	public class Customer
	{
		public Customer()
		{
			this.Orders = new List<Order>();
		}
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Order> Orders { get; set; }
	}
}
