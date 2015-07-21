using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH364
{
	[Serializable]
	public class Invoice
	{
		private int _Id;
		public virtual int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private string _Number;
		public virtual string Number
		{
			get { return _Number; }
			set { _Number = value; }
		}

		private IList<InvoiceItem> _Items = new List<InvoiceItem>();
		public virtual IList<InvoiceItem> Items
		{
			get { return _Items; }
			set { _Items = value; }
		}
	}

	[Serializable]
	public class InvoiceItem
	{
		public InvoiceItem() { }

		public InvoiceItem(Product product, decimal quantity)
		{
			this.Product = product;
			this.Quantity = quantity;
		}

		private Product _Product;
		public virtual Product Product
		{
			get { return _Product; }
			set { _Product = value; }
		}

		private decimal _Quantity;
		public virtual decimal Quantity
		{
			get { return _Quantity; }
			set { _Quantity = value; }
		}

		public bool Equals(InvoiceItem other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._Quantity == _Quantity && Equals(other._Product, _Product);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(InvoiceItem)) return false;
			return Equals((InvoiceItem)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_Quantity.GetHashCode() * 397) ^ (_Product != null ? _Product.GetHashCode() : 0);
			}
		}
	}

	[Serializable]
	public class Product
	{
		public Product() { }
		public Product(string name)
		{
			this.Name = name;
		}

		private int _Id;
		public virtual int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private string _Name;
		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public bool Equals(Product other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._Id == _Id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Product)) return false;
			return Equals((Product) obj);
		}

		public override int GetHashCode()
		{
			return _Id;
		}
	}
}
