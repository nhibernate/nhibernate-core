using System;
using System.Collections.Generic;

namespace NHibernate.Test.CompositeId
{
	public class Order
	{
		public class ID
		{
			private string customerId;
			private int orderNumber;
			public ID() {}

			public ID(string customerId, int orderNumber)
			{
				this.customerId = customerId;
				this.orderNumber = orderNumber;
			}

			public string CustomerId
			{
				get { return customerId; }
				set { customerId = value; }
			}

			public int OrderNumber
			{
				get { return orderNumber; }
				set { orderNumber = value; }
			}

			public override bool Equals(object obj)
			{
				ID that = obj as ID;
				if (that == null)
					return false;
				return customerId == that.customerId && orderNumber == that.orderNumber;
			}

			public override int GetHashCode()
			{
				return (customerId != null ? customerId.GetHashCode() : 37) ^ orderNumber.GetHashCode();
			}
		}

		private ID id = new ID();
		private DateTime orderDate;
		private Customer customer;
		private IList<LineItem> lineItems = new List<LineItem>();
		private decimal total;

		public Order() {}
		public Order(Customer customer)
		{
			this.customer = customer;
			id.CustomerId = customer.CustomerId;
			id.OrderNumber = customer.Orders.Count;
			customer.Orders.Add(this);
		}

		public virtual ID Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual DateTime OrderDate
		{
			get { return orderDate; }
			set { orderDate = value; }
		}

		public virtual Customer Customer
		{
			get { return customer; }
			set { customer = value; }
		}

		public virtual IList<LineItem> LineItems
		{
			get { return lineItems; }
			set { lineItems = value; }
		}

		public virtual decimal Total
		{
			get { return total; }
			set { total = value; }
		}

		public virtual LineItem GenerateLineItem(Product product, int quantity)
		{
			LineItem li = new LineItem(this, product);
			li.Quantity= quantity;
			lineItems.Add(li);
			return li;
		}
	}
}