using System.Linq;
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

			orders = db.Orders.Where(o => o.OrderLines.Any(ol => ol.Order == null)).ToList();
			Assert.AreEqual(0, orders.Count);
		}

		[Test]
		public void NestedAny()
		{
			var test = (from c in db.Customers
									where c.ContactName == "Bob" &&
														(c.CompanyName == "NormalooCorp" ||
														 c.Orders.Any(o => o.OrderLines.Any(ol => ol.Discount < 20 && ol.Discount >= 10)))
									select c).ToList();
			Assert.AreEqual(0, test.Count);
		}

		[Test]
		public void ManyToManyAny()
		{
			var test = db.Orders.Where(o => o.Employee.FirstName == "test");
			var result = test.Where(o => o.Employee.Territories.Any(t => t.Description == "test")).ToList();

			Assert.AreEqual(0, result.Count);
		}
	}
}
