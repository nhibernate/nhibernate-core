using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH555
{
	public enum OrderStatus
	{
		Pending = 1
	}

	public class Article
	{
		private int _id;
		private string _name;
		private decimal _price;

		public int Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public decimal Price
		{
			get { return _price; }
			set { _price = value; }
		}

		#region Constructors

		public Article()
			: this(-1, string.Empty, 0.0M)
		{
		}

		public Article(int id, string name, decimal price)
		{
			_id = id;
			_name = name;
			_price = price;
		}

		#endregion
	}

	public class Customer
	{
		private int _id;
		private string _name;

		public int Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#region Constructors

		public Customer()
		{
			_id = -1;
			_name = string.Empty;
		}

		public Customer(int id, string name)
		{
			_id = id;
			_name = name;
		}

		#endregion

		public Order CreateNewOrder()
		{
			return new Order(this);
		}
	}

	public class Order
	{
		private int _id;
		private DateTime _orderDate;
		private Customer _owningCustomer;
		private IList<OrderLine> _orderLines = new List<OrderLine>();
		private decimal _discount = 0M;
		private OrderStatus _status = OrderStatus.Pending;

		public int Id
		{
			get { return _id; }
		}

		public DateTime OrderDate
		{
			get { return _orderDate; }
		}

		public Customer OwningCustomer
		{
			get { return _owningCustomer; }
			set { _owningCustomer = value; }
		}

		public IList<OrderLine> OrderLines
		{
			get { return _orderLines; }
		}

		public decimal Discount
		{
			get { return _discount; }
		}

		public OrderStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		private Order()
		{
		}

		internal Order(Customer owningCustomer)
			: this(-1, DateTime.Now, owningCustomer)
		{
		}

		public Order(int id, DateTime orderDate, Customer owningCustomer)
		{
			_id = id;
			_orderDate = orderDate;
			_owningCustomer = owningCustomer;
		}

		public OrderLine CreateNewOrderLine()
		{
			return new OrderLine(this);
		}

		public void AddOrderLine(OrderLine ol)
		{
			ol.OwningOrder = this;
			this._orderLines.Add(ol);
		}
	}

	public class OrderLine
	{
		private int _id = -1;
		private int _articleId;
		private string _articleName;
		private decimal _articlePrice;
		private int _numberOfItems;
		private Order _owningOrder;

		public int Id
		{
			get { return _id; }
		}

		public int ArticleId
		{
			get { return _articleId; }
		}

		public string ArticleName
		{
			get { return _articleName; }
		}

		public decimal ArticlePrice
		{
			get { return _articlePrice; }
		}

		public int NumberOfItems
		{
			get { return _numberOfItems; }
			set { _numberOfItems = value; }
		}

		public decimal LineTotal
		{
			get { return _numberOfItems * _articlePrice; }
		}

		public Order OwningOrder
		{
			get { return _owningOrder; }
			set { _owningOrder = value; }
		}

		public void SetArticle(Article art)
		{
			if (art != null)
			{
				//_isArticleSet = true;
				_articleId = art.Id;
				_articleName = art.Name;
				_articlePrice = art.Price;
			}
			else
			{
				//_isArticleSet = false;
				_articleId = -1;
				_articleName = string.Empty;
				_articlePrice = 0.0M;
			}
		}

		private OrderLine()
		{
		}

		public OrderLine(Order owningOrder)
		{
			_owningOrder = owningOrder;
			_id = -1;
			_articlePrice = 0.0M;
			_articleName = string.Empty;
			_articleId = -1;
			_numberOfItems = 0;
		}
	}
}