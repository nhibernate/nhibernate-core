using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class CountTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		}

		[Test]
		public void CountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatProperty()
		{
			if (!TestDialect.SupportsCountDistinct)
				Assert.Ignore("Dialect does not support count distinct");
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.Distinct()
				.Count();

			Assert.That(result, Is.EqualTo(387));
		}

		//NH-3249 (GH-1285)
		[Test]
		public void CountDistinctFunc_ReturnsNumberOfDistinctEntriesForThatFunc()
		{
			if (!TestDialect.SupportsCountDistinct)
				Assert.Ignore("Dialect does not support count distinct");

			var result = db.Orders
				.Select(x => x.ShippingDate.Value.Date)
				.Distinct()
				.Count();

			Assert.That(result, Is.EqualTo(387));
		}

		[Test]
		public void CountProperty_ReturnsNumberOfNonNullEntriesForThatProperty()
		{
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.Count();

			Assert.That(result, Is.EqualTo(809));
		}

		[Test]
		public void Count_ReturnsNumberOfRecords()
		{
			//NH-2722
			var result = db.Orders
				.Count();

			Assert.That(result, Is.EqualTo(830));
		}

		[Test]
		public void LongCountDistinctProperty_ReturnsNumberOfDistinctEntriesForThatProperty()
		{
			if (!TestDialect.SupportsCountDistinct)
				Assert.Ignore("Dialect does not support count distinct");
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.Distinct()
				.LongCount();

			Assert.That(result, Is.EqualTo(387));
		}

		[Test]
		public void LongCountProperty_ReturnsNumberOfNonNullEntriesForThatProperty()
		{
			//NH-2722
			var result = db.Orders
				.Select(x => x.ShippingDate)
				.LongCount();

			Assert.That(result, Is.EqualTo(809));
		}

		[Test]
		public void LongCount_ReturnsNumberOfRecords()
		{
			//NH-2722
			var result = db.Orders
				.LongCount();

			Assert.That(result, Is.EqualTo(830));
		}

		[Test]
		public void CountOnJoinedGroupBy()
		{
			//NH-3001
			var query = from o in db.Orders
						join ol in db.OrderLines
						on o equals ol.Order
						group ol by ol.Product.ProductId
							into temp
							select new { temp.Key, count = temp.Count() };

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(77));
		}

		[Test]
		public void CheckSqlFunctionNameLongCount()
		{
			var name = Dialect is MsSql2000Dialect ? "count_big" : "count";
			using (var sqlLog = new SqlLogSpy())
			{
				var result = db.Orders.LongCount();
				Assert.That(result, Is.EqualTo(830));

				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Contain($"{name}("));
			}
		}

		[Test]
		public void CheckSqlFunctionNameForCount()
		{
			using (var sqlLog = new SqlLogSpy())
			{
				var result = db.Orders.Count();
				Assert.That(result, Is.EqualTo(830));

				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Contain("count("));
			}
		}

		[Test]
		public void CheckMssqlCountCast()
		{
			if (!(Dialect is MsSql2000Dialect))
			{
				Assert.Ignore();
			}

			using (var sqlLog = new SqlLogSpy())
			{
				var result = db.Orders.Count();
				Assert.That(result, Is.EqualTo(830));

				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Not.Contain("cast("));
			}
		}
	}
}
