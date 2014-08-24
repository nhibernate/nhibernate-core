using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1837
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public Customer Customer { get; set; }
	}
}
