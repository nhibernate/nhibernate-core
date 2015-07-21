namespace NHibernate.Test.CompositeId
{
	public class LineItem
	{
		public class ID
		{
			private string customerId;
			private int orderNumber;
			private string productId;
			public ID() {}
			public ID(string customerId, int orderNumber, string productId)
			{
				this.customerId = customerId;
				this.orderNumber = orderNumber;
				this.productId = productId;
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

			public string ProductId
			{
				get { return productId; }
				set { productId = value; }
			}

			public override bool Equals(object obj)
			{
				ID that = obj as ID;
				if (that == null)
					return false;

				return customerId == that.customerId && productId == that.productId && orderNumber == that.orderNumber;
			}

			public override int GetHashCode()
			{
				return (customerId != null ? customerId.GetHashCode() : 37) ^
					(productId != null ? productId.GetHashCode() : 31) ^ 
					orderNumber.GetHashCode();
			}
		}
		private ID id = new ID();
		private int quantity;
		private Order order;
		private Product product;

		public LineItem() {}
		public LineItem(Order order, Product product)
		{
			this.order = order;
			this.product = product;
			id.OrderNumber = order.Id.OrderNumber;
			id.CustomerId = order.Id.CustomerId;
			id.ProductId = product.ProductId;
			order.LineItems.Add(this);
		}

		public virtual ID Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public virtual Order Order
		{
			get { return order; }
			set { order = value; }
		}

		public virtual Product Product
		{
			get { return product; }
			set { product = value; }
		}
	}
}