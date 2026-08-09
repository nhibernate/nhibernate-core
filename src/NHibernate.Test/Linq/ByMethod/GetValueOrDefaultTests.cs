using System;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class GetValueOrDefaultTests : LinqTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// It seems that SQLite has a nasty bug with coalesce

			// Following query does not work
			//    SELECT order0_.*
			//    FROM   Orders order0_ 
			//    WHERE  coalesce(order0_.Freight, 0) > @p0;

			// And this one works
			//    SELECT order0_.*
			//    FROM   Orders order0_ 
			//    WHERE  cast(coalesce(order0_.Freight, 0) as NUMERIC) > @p0;

			if (dialect is SQLiteDialect)
				return false;
			return base.AppliesTo(dialect);
		}

		[Test]
		public void CoalesceInWhere()
		{
			var orders = db.Orders
						   .Where(x => (x.Freight ?? 100) > 0)
						   .ToList();

			Assert.AreEqual(830, orders.Count);
		}

		[Test]
		public void GetValueOrDefaultInWhere()
		{
			var orders = db.Orders
						   .Where(x => x.Freight.GetValueOrDefault(100) > 0)
						   .ToList();

			Assert.AreEqual(830, orders.Count);
		}

		[Test]
		public void GetValueOrDefaultWithSingleArgumentInWhere()
		{
			var orders = db.Orders
						   .Where(x => x.Freight.GetValueOrDefault() > 0)
						   .ToList();

			Assert.AreEqual(830, orders.Count);
		}

		[Test]
		public void GetValueOrDefaultOnDateTimeInSelect()
		{
			var dates = db.Orders
						  .Select(x => x.ShippingDate.GetValueOrDefault())
						  .ToList();

			Assert.That(dates, Has.Count.EqualTo(830));
			Assert.That(dates, Has.Some.EqualTo(default(DateTime)), "Orders without a shipping date should default.");
		}

		[Test]
		public void GetValueOrDefaultOnDateTimeInWhere()
		{
			var orders = db.Orders
						   .Where(x => x.ShippingDate.GetValueOrDefault() > new DateTime(1990, 1, 1))
						   .ToList();

			Assert.That(orders, Has.Count.EqualTo(db.Orders.Count(x => x.ShippingDate != null)));
		}

		[Test]
		public void GetValueOrDefaultOnDateTimeInOrderBy()
		{
			var orders = db.Orders
						   .OrderByDescending(x => x.ShippingDate.GetValueOrDefault())
						   .ToList();

			Assert.That(orders, Has.Count.EqualTo(830));
		}

		[Test]
		public void GetValueOrDefaultOnEnumStoredAsString()
		{
			using (var sqlLog = new SqlLogSpy())
			{
				var users = db.Users
							  .Where(x => x.NullableEnum1.GetValueOrDefault() == EnumStoredAsString.Medium)
							  .ToList();

				Assert.That(users, Has.Count.EqualTo(2));
				// The default value must be sent as a string, as the member is mapped as one.
				Assert.That(sqlLog.GetWholeLog(), Does.Contain(nameof(EnumStoredAsString.Unspecified)));
			}
		}

		[Test]
		public void GetValueOrDefaultOnEnumStoredAsInt32()
		{
			var users = db.Users
						  .Where(x => x.NullableEnum2.GetValueOrDefault() == EnumStoredAsInt32.Unspecified)
						  .ToList();

			Assert.That(users, Has.Count.EqualTo(2));
		}
	}
}
