using System;
using System.Collections;
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

		private IList _Items = new ArrayList();
		public virtual IList Items
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
	}
}
