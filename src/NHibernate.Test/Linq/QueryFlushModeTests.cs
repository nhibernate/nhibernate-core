using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryFlushModeTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			base.Configure(configuration);
		}

		[Test]
		public void CanSetFlushModeOnQueries(
			[Values(FlushMode.Always, FlushMode.Auto, FlushMode.Commit, FlushMode.Manual)]
			FlushMode flushMode)
		{
			Sfi.Statistics.Clear();

			using (var t = session.BeginTransaction())
			{
				var customer = db.Customers.First();
				customer.CompanyName = "Blah";

				var unused =
					db.Customers
					  .Where(c => c.CompanyName == "Bon app'")
					  .WithOptions(o => o.SetFlushMode(flushMode))
					  .ToList();

				var expectedFlushCount = 0;
				switch (flushMode)
				{
					case FlushMode.Always:
					case FlushMode.Auto:
						expectedFlushCount++;
						break;
				}
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(expectedFlushCount), "Unexpected flush count on same entity query");

				customer.CompanyName = "Other blah";

				var dummy =
					db.Orders
					  .Where(o => o.OrderId > 10)
					  .WithOptions(o => o.SetFlushMode(flushMode))
					  .ToList();

				switch (flushMode)
				{
					case FlushMode.Always:
						expectedFlushCount++;
						break;
				}
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(expectedFlushCount), "Unexpected flush count on other entity query");

				// Tests here should not alter data, LinqTestCase derives from ReadonlyTestCase
				t.Rollback();
			}
		}

		[Test]
		public void CanSetCommentOnPagingQuery(
			[Values(FlushMode.Always, FlushMode.Auto, FlushMode.Commit, FlushMode.Manual)]
			FlushMode flushMode)
		{
			Sfi.Statistics.Clear();

			using (var t = session.BeginTransaction())
			{
				var customer = db.Customers.First();
				customer.CompanyName = "Blah";

				var unused =
					db.Customers
					  .Skip(1).Take(1)
					  .WithOptions(o => o.SetFlushMode(flushMode))
					  .ToList();

				var expectedFlushCount = 0;
				switch (flushMode)
				{
					case FlushMode.Always:
					case FlushMode.Auto:
						expectedFlushCount++;
						break;
				}
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(expectedFlushCount), "Unexpected flush count on same entity query");

				customer.CompanyName = "Other blah";

				var dummy =
					db.Orders
					  .Skip(1).Take(1)
					  .WithOptions(o => o.SetFlushMode(flushMode))
					  .ToList();

				switch (flushMode)
				{
					case FlushMode.Always:
						expectedFlushCount++;
						break;
				}
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(expectedFlushCount), "Unexpected flush count on other entity query");

				// Tests here should not alter data, LinqTestCase derives from ReadonlyTestCase
				t.Rollback();
			}
		}

		[Test]
		public void CanSetCommentBeforeSkipOnOrderedPageQuery(
			[Values(FlushMode.Always, FlushMode.Auto, FlushMode.Commit, FlushMode.Manual)]
			FlushMode flushMode)
		{
			Sfi.Statistics.Clear();

			using (var t = session.BeginTransaction())
			{
				var customer = db.Customers.First();
				customer.CompanyName = "Blah";

				var unused =
					db.Customers
					  .OrderBy(c => c.CompanyName)
					  .Skip(5).Take(5)
					  .WithOptions(o => o.SetFlushMode(flushMode))
					  .ToList();

				var expectedFlushCount = 0;
				switch (flushMode)
				{
					case FlushMode.Always:
					case FlushMode.Auto:
						expectedFlushCount++;
						break;
				}
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(expectedFlushCount), "Unexpected flush count on same entity query");

				customer.CompanyName = "Other blah";

				var dummy =
					db.Orders
					  .OrderBy(o => o.OrderId)
					  .Skip(5).Take(5)
					  .WithOptions(o => o.SetFlushMode(flushMode))
					  .ToList();

				switch (flushMode)
				{
					case FlushMode.Always:
						expectedFlushCount++;
						break;
				}
				Assert.That(Sfi.Statistics.FlushCount, Is.EqualTo(expectedFlushCount), "Unexpected flush count on other entity query");

				// Tests here should not alter data, LinqTestCase derives from ReadonlyTestCase
				t.Rollback();
			}
		}
	}
}
