using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class Thing
	{
		public int ProductId { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Other { get; set; }

		public Thing(int productId, decimal? unitPrice, decimal? other)
		{
			ProductId = productId;
			UnitPrice = unitPrice;
			Other = other;
		}
	}

	public class Product
	{
		private readonly IList<OrderLine> _orderLines;
		private ProductCategory _category;
		private bool _discontinued;
		private string _quantityPerUnit;
		private int _reorderLevel;
		private Supplier _supplier;
		private decimal? _unitPrice;
		private int _unitsInStock;
		private int _unitsOnOrder;
		private int _productId;
		private string _name;
		private float _shippingWeight;

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

		public virtual Supplier Supplier
		{
			get { return _supplier; }
			set { _supplier = value; }
		}

		public virtual ProductCategory Category
		{
			get { return _category; }
			set { _category = value; }
		}

		public virtual string QuantityPerUnit
		{
			get { return _quantityPerUnit; }
			set { _quantityPerUnit = value; }
		}

		public virtual decimal? UnitPrice
		{
			get { return _unitPrice; }
			set { _unitPrice = value; }
		}

		public virtual int UnitsInStock
		{
			get { return _unitsInStock; }
			set { _unitsInStock = value; }
		}

		public virtual int UnitsOnOrder
		{
			get { return _unitsOnOrder; }
			set { _unitsOnOrder = value; }
		}

		public virtual int ReorderLevel
		{
			get { return _reorderLevel; }
			set { _reorderLevel = value; }
		}

		public virtual bool Discontinued
		{
			get { return _discontinued; }
			set { _discontinued = value; }
		}

		public virtual float ShippingWeight
		{
			get { return _shippingWeight; }
			set { _shippingWeight = value; }
		}

		public virtual ReadOnlyCollection<OrderLine> OrderLines
		{
			get { return new ReadOnlyCollection<OrderLine>(_orderLines); }
		}
	}
}