using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3607
{
	public class Order
	{
		public virtual int Id { get; set; }

		public virtual DateTime CreatedDate { get; set; }

		public virtual ISet<LineItem> Items { get; protected set; } = new HashSet<LineItem>();
	}
}
