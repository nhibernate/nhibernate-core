using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class AnyTests : LinqTestCase
	{
		[Test]
		public void AnySublist()
		{
			var orders = db.Orders.Where(o => o.OrderLines.Any(ol => ol.Quantity == 5)).ToList();
			Assert.AreEqual(61, orders.Count);
		}
	}
}
