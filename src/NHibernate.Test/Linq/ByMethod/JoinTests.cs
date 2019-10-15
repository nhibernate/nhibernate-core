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
	}
}
