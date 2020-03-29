using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine.Query;
using NHibernate.Util;
using NSubstitute;
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

		[TestCase(false)]
		[TestCase(true)]
		public void CrossJoinWithPredicateInWhereStatement(bool useCrossJoin)
		{
			if (useCrossJoin && !Dialect.SupportsCrossJoin)
			{
				Assert.Ignore("Dialect does not support cross join.");
			}

			using (var substitute = SubstituteDialect())
			using (var sqlSpy = new SqlLogSpy())
			{
				ClearQueryPlanCache();
				substitute.Value.SupportsCrossJoin.Returns(useCrossJoin);

				var result = (from o in db.Orders
							from o2 in db.Orders.Where(x => x.Freight > 50)
							where (o.OrderId == o2.OrderId + 1) || (o.OrderId == o2.OrderId - 1)
							select new { o.OrderId, OrderId2 = o2.OrderId }).ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(result.Count, Is.EqualTo(720));
				Assert.That(sql, Does.Contain(useCrossJoin ? "cross join" : "inner join"));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(useCrossJoin ? 0 : 1));
			}
		}
	}
}
