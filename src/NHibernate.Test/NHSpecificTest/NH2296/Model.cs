using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2296
{
	public class Order
	{
		public virtual int Id { get; set; }
		public virtual string AccountName { get; set; }
		public virtual IList<Product> Products { get; set; }

		public Order()
		{
			this.Products = new List<Product>();
		}
	}

	public class Product
	{
		public virtual int Id { get; set; }
		public virtual string StatusReason { get; set; }
		public virtual Order Order { get; set; }
	}
}