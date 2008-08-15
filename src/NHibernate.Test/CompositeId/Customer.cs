using System.Collections;
using System;

namespace NHibernate.Test.CompositeId
{
	public class Customer
	{
		private string customerId;
		private string name;
		private string address;
		private IList orders = new ArrayList();

		public virtual string CustomerId
		{
			get { return customerId; }
			set { customerId = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Address
		{
			get { return address; }
			set { address = value; }
		}

		public virtual IList Orders
		{
			get { return orders; }
			set { orders = value; }
		}

		public virtual Order GenerateNewOrder(decimal total)
		{
			Order order = new Order(this);
			order.OrderDate = DateTime.Today;
			order.Total=total;

			return order;
		}
	}
}