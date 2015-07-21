using System.Linq;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class AnyTests : LinqTestCase
	{
		[Test]
		public void AnySublist()
		{
			var orders = db.Orders.Where(o => o.OrderLines.Any(ol => ol.Quantity == 5)).ToList();
			Assert.That(orders.Count, Is.EqualTo(61));

			orders = db.Orders.Where(o => o.OrderLines.Any(ol => ol.Order == null)).ToList();
			Assert.That(orders.Count, Is.EqualTo(0));
		}

		[Test]
		public void NestedAny()
		{
			var test = (from c in db.Customers
						where c.ContactName == "Bob" &&
							  (c.CompanyName == "NormalooCorp" ||
							   c.Orders.Any(o => o.OrderLines.Any(ol => ol.Discount < 20 && ol.Discount >= 10)))
						select c).ToList();

			Assert.That(test.Count, Is.EqualTo(0));
		}

		[Test]
		public void ManyToManyAny()
		{
			var test = db.Orders.Where(o => o.Employee.FirstName == "test");
			var result = test.Where(o => o.Employee.Territories.Any(t => t.Description == "test")).ToList();

			Assert.That(result.Count, Is.EqualTo(0));
		}

		[Test(Description = "NH-2654")]
		public void AnyWithCount()
		{
			var result = db.Orders
				.Any(p => p.OrderLines.Count == 0);

			Assert.That(result, Is.False);
		}

		[Test]
		public void AnyWithFetch()
		{
			//NH-3241
			var result = db.Orders.Fetch(x => x.Customer).FetchMany(x => x.OrderLines).Any();
		}
	}
}
