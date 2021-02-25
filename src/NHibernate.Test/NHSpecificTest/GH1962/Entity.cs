using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1962
{
	public class Order
	{
		public virtual int Id { get; protected set; }

		public virtual IList<OrderDetail> OrderDetails { get; protected set; } = new List<OrderDetail>();
	}

	public class Product
	{
		public virtual int Id { get; protected set; }
		public virtual string Name { get; protected set; }

		public virtual IList<OrderDetail> OrderDetails { get; protected set; } = new List<OrderDetail>();
	}

	public class OrderDetail
	{
		public virtual int Id { get; protected set; }

		public virtual Order Order { get; protected set; }
		public virtual Product Product { get; protected set; }
	}
}
