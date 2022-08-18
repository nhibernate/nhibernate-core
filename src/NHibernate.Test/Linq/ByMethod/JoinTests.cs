using System.Linq;
using NHibernate.Linq;
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
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = db.Orders
						   .Join(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
						   .Select(x => new { First = x.order, Second = x.order1 })
						   .Join(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
						   .Select(x => new { FirstId = x.order.First.OrderId, SecondId = x.order.Second.OrderId, ThirdId = x.order1.OrderId })
						   .ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(828));
				Assert.IsTrue(orders.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(2));
			}
		}

		[Test]
		public void MultipleLinqJoinsWithSameProjectionNamesWithLeftJoin()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = db.Orders
								.GroupJoin(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
								.SelectMany(x => x.order1.DefaultIfEmpty(), (x, order1) => new { First = x.order, Second = order1 })
								.GroupJoin(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
								.SelectMany(x => x.order1.DefaultIfEmpty(), (x, order1) => new
								{
									FirstId = x.order.First.OrderId,
									SecondId = (int?) x.order.Second.OrderId,
									ThirdId = (int?) order1.OrderId
								})
								.ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(830));
				Assert.IsTrue(orders.Where(x => x.SecondId.HasValue && x.ThirdId.HasValue)
									.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
			}
		}

		[Test]
		public void MultipleLinqJoinsWithSameProjectionNamesWithLeftJoinExtensionMethod()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = db.Orders
								.LeftJoin(db.Orders, x => x.OrderId, x => x.OrderId - 1, (order, order1) => new { order, order1 })
								.Select(x => new { First = x.order, Second = x.order1 })
								.LeftJoin(db.Orders, x => x.First.OrderId, x => x.OrderId - 2, (order, order1) => new { order, order1 })
								.Select(x => new
								{
									FirstId = x.order.First.OrderId,
									SecondId = (int?) x.order.Second.OrderId,
									ThirdId = (int?) x.order1.OrderId
								})
								.ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(830));
				Assert.IsTrue(orders.Where(x => x.SecondId.HasValue && x.ThirdId.HasValue)
									.All(x => x.FirstId == x.SecondId - 1 && x.SecondId == x.ThirdId - 1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
			}
		}

		[Test]
		public void LeftJoinExtensionMethodWithMultipleKeyProperties()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var orders = db.Orders
				               .LeftJoin(
					               db.Orders,
					               x => new {x.OrderId, x.Customer.CustomerId},
					               x => new {x.OrderId, x.Customer.CustomerId},
					               (order, order1) => new {order, order1})
				               .Select(x => new {FirstId = x.order.OrderId, SecondId = x.order1.OrderId})
				               .ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(orders.Count, Is.EqualTo(830));
				Assert.IsTrue(orders.All(x => x.FirstId == x.SecondId));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test(Description = "GH-3104")]
		public void LeftJoinExtensionMethodWithInnerJoinAfter()
		{
			var animals = db.Animals
						   .LeftJoin(db.Mammals, o => o.Id, i => i.Id, (o, i) => new { animal = o, mammalLeft1 = i })
						   .LeftJoin(db.Mammals, x => x.mammalLeft1.Id, y => y.Id, (o, i) => new { o.animal, o.mammalLeft1, mammalLeft2 = i })
						   .Join(db.Mammals, o => o.mammalLeft2.Id, y => y.Id, (o, i) => new { o.animal, o.mammalLeft1, o.mammalLeft2, mammalInner = i })
						   .Where(x => x.mammalLeft1.SerialNumber.StartsWith("9"))
						   .Where(x => x.mammalLeft2.SerialNumber.StartsWith("9"))
						   .Where(x => x.animal.SerialNumber.StartsWith("9"))
						   .Where(x => x.mammalInner.SerialNumber.StartsWith("9"))
						   .Select(x => new { SerialNumber = x.animal.SerialNumber })
						   .ToList();

			Assert.That(animals.Count, Is.EqualTo(1));
		}

		[Test]
		public void LeftJoinExtensionMethodWithOuterReferenceInWhereClauseOnly()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var animals = db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .Where(x => x.mammal.SerialNumber.StartsWith("9"))
							   .Select(x => new { SerialNumber = x.animal.SerialNumber })
							   .ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test]
		public void LeftJoinExtensionMethodWithOuterReferenceInWhereClauseOnlyCount()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var total = db.Orders
				                .LeftJoin(
					                db.OrderLines,
					                x => x,
					                x => x.Order,
					                (order, line) => new { order, line })
				                
				                .Select(x => new { x.order.OrderId, x.line.Discount })
				                .Count();
				var sql = sqlSpy.GetWholeLog();
				Assert.That(total, Is.EqualTo(2155));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[KnownBug("GH-2739")]
		public void NestedLeftJoinExtensionMethodWithOuterReferenceInWhereClauseOnly()
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
				
				var animals = db.Animals
							   .LeftJoin(
								   innerAnimals,
								   x => x.Id,
								   x => x.Id,
								   (animal, animal2) => new { animal, animal2 })
							   .Select(x => new { SerialNumber = x.animal2.SerialNumber })
							   .ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test]
		public void LeftJoinExtensionMethodWithNoUseOfOuterReference()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var animals = db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .Select(x => x.animal)
							   .ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(6));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(6));
			}
		}

		[Test]
		public void LeftJoinExtensionMethodWithNoUseOfOuterReferenceCount()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var total = db.Animals
				              .LeftJoin(
					              db.Mammals,
					              x => x.Id,
					              x => x.Id,
					              (animal, mammal) => new {animal, mammal})
				              .Select(x => x.animal)
				              .Count();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(total, Is.EqualTo(6));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test]
		public void LeftJoinExtensionMethodWithOuterReferenceInOrderByClauseOnly()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var animals = db.Animals
							   .LeftJoin(
								   db.Mammals,
								   x => x.Id,
								   x => x.Id,
								   (animal, mammal) => new { animal, mammal })
							   .OrderBy(x => x.mammal.SerialNumber ?? "z")
							   .Select(x => new { SerialNumber = x.animal.SerialNumber })
							   .ToList();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(animals.Count, Is.EqualTo(6));
				Assert.That(animals[0].SerialNumber, Is.EqualTo("1121"));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
		}

		[Test]
		public void LeftJoinExtensionMethodWithOuterReferenceInOrderByClauseOnlyCount()
		{
			using (var sqlSpy = new SqlLogSpy())
			{
				var total = db.Animals
				              .LeftJoin(
					              db.Mammals,
					              x => x.Id,
					              x => x.Id,
					              (animal, mammal) => new {animal, mammal})
				              .OrderBy(x => x.mammal.SerialNumber ?? "z")
				              .Select(x => new {SerialNumber = x.animal.SerialNumber})
				              .Count();

				var sql = sqlSpy.GetWholeLog();
				Assert.That(total, Is.EqualTo(6));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(1));
			}
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

		[Test]
		public void CanJoinOnEntityWithSubclasses()
		{
			var result = (from o in db.Animals
						from o2 in db.Animals.Where(x => x.BodyWeight > 50)
						select new {o, o2}).Take(1).ToList();
		}

		[Test]
		public void CanInnerJoinOnEntityWithSubclasses()
		{
			//inner joined animal is not used in output (no need to join subclasses)
			var resultsFromOuter1 = db.Animals.Join(db.Animals, o => o.Id, i => i.Id, (o, i) => o).Take(1).ToList();

			//inner joined mammal is not used in output (but subclass join is needed for mammal)
			var resultsFromOuter2 = db.Animals.Join(db.Mammals, o => o.Id, i => i.Id, (o, i) => o).Take(1).ToList();

			//inner joined animal is used in output (all subclass joins are required)
			var resultsFromInner1 = db.Animals.Join(db.Animals, o => o.Id, i => i.Id, (o, i) => i).Take(1).ToList();
		}

		[Test(Description = "GH-2580")]
		public void CanInnerJoinOnSubclassWithBaseTableReferenceInOnClause()
		{
			var result = (from o in db.Animals
			              join o2 in db.Mammals on o.BodyWeight equals o2.BodyWeight
			              select new { o, o2 }).Take(1).ToList();
		}

		[Test(Description = "GH-2805")]
		public void CanJoinOnInterface()
		{
			var result = db.IUsers.Join(db.IUsers,
									u => u.Id,
									iu => iu.Id,
									(u, iu) => iu.Name).Take(1).ToList();
		}
	}
}
