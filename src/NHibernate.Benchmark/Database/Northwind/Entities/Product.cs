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

		public Product()
		{
			_orderLines = new List<OrderLine>();
		}

		public virtual int ProductId { get; set; }

		public virtual string Name { get; set; }

		public virtual Supplier Supplier { get; set; }

		public virtual ProductCategory Category { get; set; }

		public virtual string QuantityPerUnit { get; set; }

		public virtual decimal? UnitPrice { get; set; }

		public virtual int UnitsInStock { get; set; }

		public virtual int UnitsOnOrder { get; set; }

		public virtual int ReorderLevel { get; set; }

		public virtual bool Discontinued { get; set; }

		public virtual float ShippingWeight { get; set; }

		public virtual ReadOnlyCollection<OrderLine> OrderLines => new ReadOnlyCollection<OrderLine>(_orderLines);
	}
}