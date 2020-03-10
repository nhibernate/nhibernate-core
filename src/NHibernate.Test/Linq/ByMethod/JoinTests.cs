using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class JoinTests : LinqTestCase
	{
		[Test]
		public void MultipleLinqJoinsWithSameProjectionNames()
		{
			var orders = db.Orders
						   .Join(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
						   .Select(x => new { First = x.order, Second = x.order1 })
						   .Join(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
						   .Select(x => new { FirstId = x.order.First.OrderId, SecondId = x.order.Second.OrderId, ThirdId = x.order1.OrderId })
						   .ToList();

			Assert.That(orders.Count, Is.EqualTo(828));
			Assert.IsTrue(orders.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
		}

		[Test]
		public void CrossJoinWithPredicateInOnStatement()
		{
			var result =
				(from o in db.Orders
				from p in db.Products
				join d in db.OrderLines
					on new { o.OrderId, p.ProductId } equals new { d.Order.OrderId, d.Product.ProductId }
					into details
				from d in details
				select new { o.OrderId, p.ProductId, d.UnitPrice }).Take(10).ToList();

			Assert.That(result.Count, Is.EqualTo(10));
		}

		[Test]
		public void CrossJoinWithPredicateInWhereStatement()
		{
			var result = (from o in db.Orders
						from o2 in db.Orders.Where(x => x.Freight > 50)
						where (o.OrderId == o2.OrderId + 1) || (o.OrderId == o2.OrderId - 1)
						select new { o.OrderId, OrderId2 = o2.OrderId }).ToList();

			Assert.That(result.Count, Is.EqualTo(720));
		}
	}
}
