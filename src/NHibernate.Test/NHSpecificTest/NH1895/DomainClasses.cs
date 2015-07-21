using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1895
{
	public class Order
	{
		public Order()
		{
			Details = new List<Detail>();
		}

		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<Detail> Details { get; set; }
	}

	public class Detail
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Order Parent { get; set; }
	}
}
