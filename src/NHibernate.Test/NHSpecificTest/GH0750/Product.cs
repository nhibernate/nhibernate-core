using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.Test.NHSpecificTest.GH0750
{
	public class Product
	{
		private readonly IList<OrderLine> _orderLines;
		private decimal? _unitPrice;
		private int _productId;
		private string _name;

		public Product()
		{
			_orderLines = new List<OrderLine>();
		}

		public virtual int ProductId
		{
			get { return _productId; }
			set { _productId = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual decimal? UnitPrice
		{
			get { return _unitPrice; }
			set { _unitPrice = value; }
		}

		public virtual ReadOnlyCollection<OrderLine> OrderLines
		{
			get { return new ReadOnlyCollection<OrderLine>(_orderLines); }
		}
	}
}
