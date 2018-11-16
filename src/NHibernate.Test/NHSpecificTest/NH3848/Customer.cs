using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class Customer
	{
		public virtual Guid Id { get; set; }
		public virtual ISet<Order> Orders { get; set; } = new HashSet<Order>();
		public virtual ISet<Company> Companies { get; set; } = new HashSet<Company>();
		public virtual string Name { get; set; }

		public virtual void AddOrder(Order order)
		{
			Orders.Add(order);
			order.Customer = this;
		}

		public virtual void AddCompany(Company company)
		{
			Companies.Add(company);
			company.Customer = this;
		}
	}
}
