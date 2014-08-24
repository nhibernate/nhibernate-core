using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2736
{
	public class SalesOrder
	{
		private IList<Item> m_Items = new List<Item>();

		public virtual Int32 Id { get; set; }
		public virtual Int32 Number { get; set; }

		public virtual IList<Item> Items
		{
			get { return m_Items; }
			set { m_Items = value; }
		}
	}

	public class Item
	{
		public virtual Int32 Id { get; set; }
		public virtual SalesOrder SalesOrder { get; set; }
		public virtual Int32 Quantity { get; set; }
	}
}
