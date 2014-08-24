using System;

namespace NHibernate.Test.FilterTest
{
	public class LineItem
	{
		private long id;
		private Order order;
		private int sequence;
		private Product product;
		private long quantity;

		protected internal LineItem()
		{
		}

		public static LineItem generate(Order order, int sequence, Product product, long quantity)
		{
			LineItem item = new LineItem();
			item.order = order;
			item.sequence = sequence;
			item.product = product;
			item.quantity = quantity;
			item.order.LineItems.Insert(sequence, item);
			return item;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Order Order
		{
			get { return order; }
			set { order = value; }
		}

		public virtual int Sequence
		{
			get { return sequence; }
			set { sequence = value; }
		}

		public virtual Product Product
		{
			get { return product; }
			set { product = value; }
		}

		public virtual long Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}
	}
}