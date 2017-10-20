namespace NHibernate.Test.NHSpecificTest.GH0750
{
	public class OrderLine
	{
		private int _orderLineId;
		private Order _order;
		private Product _product;
		private int _quantity;
		private decimal _unitPrice;

		public OrderLine() : this(null, null)
		{
		}

		public OrderLine(Order order, Product product)
		{
			_order = order;
			_product = product;
		}

		public virtual int OrderLineId
		{
			get { return _orderLineId; }
			set { _orderLineId = value; }
		}

		public virtual Order Order
		{
			get { return _order; }
			set { _order = value; }
		}

		public virtual Product Product
		{
			get { return _product; }
			set { _product = value; }
		}

		public virtual decimal UnitPrice
		{
			get { return _unitPrice; }
			set { _unitPrice = value; }
		}

		public virtual int Quantity
		{
			get { return _quantity; }
			set { _quantity = value; }
		}
	}
}
