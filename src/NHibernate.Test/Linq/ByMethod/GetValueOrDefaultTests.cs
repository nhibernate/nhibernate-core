using System.Linq;
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
	}
}
