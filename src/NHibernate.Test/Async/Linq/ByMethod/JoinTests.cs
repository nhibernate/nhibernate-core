﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Linq;
using NHibernate.Util;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class JoinTestsAsync : LinqTestCase
	{
		[Test]
		public async Task MultipleLinqJoinsWithSameProjectionNamesAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = await (db.Orders
						   .Join(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
						   .Select(x => new { First = x.order, Second = x.order1 })
						   .Join(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
						   .Select(x => new { FirstId = x.order.First.OrderId, SecondId = x.order.Second.OrderId, ThirdId = x.order1.OrderId })
						   .ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(828));
				Assert.IsTrue(orders.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(2));
			}
		}

		[Test]
		public async Task MultipleLinqJoinsWithSameProjectionNamesWithLeftJoinAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = await (db.Orders
								.GroupJoin(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
								.SelectMany(x => x.order1.DefaultIfEmpty(), (x, order1) => new { First = x.order, Second = order1 })
								.GroupJoin(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
								.SelectMany(x => x.order1.DefaultIfEmpty(), (x, order1) => new
								{
									FirstId = x.order.First.OrderId,
									SecondId = (int?) x.order.Second.OrderId,
									ThirdId = (int?) order1.OrderId
								})
								.ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(830));
				Assert.IsTrue(orders.Where(x => x.SecondId.HasValue && x.ThirdId.HasValue)
									.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
			}
		}

		[Test]
		public async Task MultipleLinqJoinsWithSameProjectionNamesWithLeftJoinExtensionMethodAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = await (db.Orders
								.LeftJoin(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
								.Select(x => new { First = x.order, Second = x.order1 })
								.LeftJoin(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
								.Select(x => new
								{
									FirstId = x.order.First.OrderId,
									SecondId = (int?) x.order.Second.OrderId,
									ThirdId = (int?) x.order1.OrderId
								})
								.ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(830));
				Assert.IsTrue(orders.Where(x => x.SecondId.HasValue && x.ThirdId.HasValue)
									.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
			}
		}

		[Test]
		public async Task LeftJoinExtensionMethodWithMultipleKeyPropertiesAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = await (db.Orders
				               .LeftJoin(
					               db.Orders,
					               x => new {x.OrderId, x.Customer.CustomerId},
					               x => new {x.OrderId, x.Customer.CustomerId},
					               (order, order1) => new {order, order1})
				               .Select(x => new {FirstId = x.order.OrderId, SecondId = x.order1.OrderId})
				               .ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(830));
				Assert.IsTrue(orders.All(x => x.FirstId == x.SecondId));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test]
		public async Task LeftJoinExtensionMethodWithOuterReferenceInWhereClauseOnlyAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var animals = await (db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .Where(x => x.mammal.SerialNumber.StartsWith("9"))
							   .Select(x => new { SerialNumber = x.animal.SerialNumber })
							   .ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[KnownBug("GH-2379")]
		public async Task NestedLeftJoinExtensionMethodWithOuterReferenceInWhereClauseOnlyAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var innerAnimals = db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .Where(x => x.mammal.SerialNumber.StartsWith("9"))
							   .Select(x=>x.animal);
				
				var animals = await (db.Animals
							   .LeftJoin(
								   innerAnimals,
								   x => x.Id,
								   x => x.Id,
								   (animal, animal2) => new { animal, animal2 })
							   .Select(x => new { SerialNumber = x.animal2.SerialNumber })
							   .ToListAsync(cancellationToken));

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test]
		public async Task LeftJoinExtensionMethodWithNoUseOfOuterReferenceAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var animals = await (db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .Select(x => x.animal)
							   .ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(6));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(5));
			}
		}

		[Test]
		public async Task LeftJoinExtensionMethodWithOuterReferenceInOrderByClauseOnlyAsync()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var animals = await (db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .OrderBy(x => x.mammal.SerialNumber ?? "z")
							   .Select(x => new { SerialNumber = x.animal.SerialNumber })
							   .ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(6));
				Assert.That(animals[0].SerialNumber, Is.EqualTo("1121"));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public async Task CrossJoinWithPredicateInWhereStatementAsync(bool useCrossJoin)
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

				var result = await ((from o in db.Orders
							from o2 in db.Orders.Where(x => x.Freight > 50)
							where (o.OrderId == o2.OrderId + 1) || (o.OrderId == o2.OrderId - 1)
							select new { o.OrderId, OrderId2 = o2.OrderId }).ToListAsync());

				var sql = sqlSpy.GetWholeLog();
				Assert.That(result.Count, Is.EqualTo(720));
				Assert.That(sql, Does.Contain(useCrossJoin ? "cross join" : "inner join"));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(useCrossJoin ? 0 : 1));
			}
		}

		[Test]
		public async Task CanJoinOnEntityWithSubclassesAsync()
		{
			var result = await ((from o in db.Animals
						from o2 in db.Animals.Where(x => x.BodyWeight > 50)
						select new {o, o2}).Take(1).ToListAsync());
		}

		[Test(Description = "GH-2580")]
		public async Task CanInnerJoinOnSubclassWithBaseTableReferenceInOnClauseAsync()
		{
			var result = await ((from o in db.Animals
			              join o2 in db.Mammals on o.BodyWeight equals o2.BodyWeight
			              select new { o, o2 }).Take(1).ToListAsync());
		}
	}
}
