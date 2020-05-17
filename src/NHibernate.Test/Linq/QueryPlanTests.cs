using System.Linq;
using NHibernate.Dialect;
using NHibernate.Linq;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryPlanTests : LinqTestCase
	{
		[Test]
		public void SelectConstantShouldBeCached()
		{
			ClearQueryPlanCache();

			var c1 = db.Customers.Select(o => new {o.CustomerId, Constant = "constant"}).First();
			var c2 = db.Customers.Select(o => new {o.CustomerId, Constant = "constant2"}).First();
			var constant = "constant3";
			var c3 = db.Customers.Select(o => new {o.CustomerId, Constant = constant}).First();
			constant = "constant4";
			var c4 = db.Customers.Select(o => new {o.CustomerId, Constant = constant}).First();

			var queryCache = GetQueryPlanCache();
			Assert.That(queryCache.Count, Is.EqualTo(1));

			Assert.That(c1.Constant, Is.EqualTo("constant"));
			Assert.That(c2.Constant, Is.EqualTo("constant2"));
			Assert.That(c3.Constant, Is.EqualTo("constant3"));
			Assert.That(c4.Constant, Is.EqualTo("constant4"));
		}

		[Test]
		public void GroupByConstantShouldBeCached()
		{
			ClearQueryPlanCache();

			var c1 = db.Customers.GroupBy(o => new {o.CustomerId, Constant = "constant"}).Select(o => o.Key).First();
			var c2 = db.Customers.GroupBy(o => new {o.CustomerId, Constant = "constant2"}).Select(o => o.Key).First();
			var constant = "constant3";
			var c3 = db.Customers.GroupBy(o => new {o.CustomerId, Constant = constant}).Select(o => o.Key).First();
			constant = "constant4";
			var c4 = db.Customers.GroupBy(o => new {o.CustomerId, Constant = constant}).Select(o => o.Key).First();

			var queryCache = GetQueryPlanCache();
			Assert.That(queryCache.Count, Is.EqualTo(1));

			Assert.That(c1.Constant, Is.EqualTo("constant"));
			Assert.That(c2.Constant, Is.EqualTo("constant2"));
			Assert.That(c3.Constant, Is.EqualTo("constant3"));
			Assert.That(c4.Constant, Is.EqualTo("constant4"));
		}

		[Test]
		public void WithLockShouldBeCached()
		{
			ClearQueryPlanCache();
			// Limit to a few dialects where we know the "nowait" keyword is used to make life easier.
			Assume.That(Dialect is MsSql2000Dialect || Dialect is Oracle8iDialect || Dialect is PostgreSQL81Dialect);

			db.Customers.WithLock(LockMode.Upgrade).ToList();
			db.Customers.WithLock(LockMode.UpgradeNoWait).ToList();
			var lockMode = LockMode.None;
			db.Customers.WithLock(lockMode).ToList();
			lockMode = LockMode.Read;
			db.Customers.WithLock(lockMode).ToList();

			var queryCache = GetQueryPlanCache();
			Assert.That(queryCache.Count, Is.EqualTo(4));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void SkipShouldBeCached(bool supportsVariableLimit)
		{
			if (!Dialect.SupportsLimit || (supportsVariableLimit && !Dialect.SupportsVariableLimit))
			{
				Assert.Ignore();
			}

			ClearQueryPlanCache();
			using (var substitute = SubstituteDialect())
			{
				substitute.Value.Configure().SupportsVariableLimit.Returns(supportsVariableLimit);

				var c1 = db.Customers.Skip(1).ToList();
				var c2 = db.Customers.Skip(2).ToList();
				var skip = 3;
				var c3 = db.Customers.Skip(skip).ToList();
				skip = 4;
				var c4 = db.Customers.Skip(skip).ToList();

				var queryCache = GetQueryPlanCache();
				Assert.That(c1.Count, Is.Not.EqualTo(c2.Count));
				Assert.That(c2.Count, Is.Not.EqualTo(c3.Count));
				Assert.That(c3.Count, Is.Not.EqualTo(c4.Count));
				Assert.That(queryCache.Count, Is.EqualTo(supportsVariableLimit ? 1 : 4));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void TakeShouldBeCached(bool supportsVariableLimit)
		{
			if (!Dialect.SupportsLimit || (supportsVariableLimit && !Dialect.SupportsVariableLimit))
			{
				Assert.Ignore();
			}

			ClearQueryPlanCache();
			using (var substitute = SubstituteDialect())
			{
				substitute.Value.Configure().SupportsVariableLimit.Returns(supportsVariableLimit);

				var c1 = db.Customers.Take(1).ToList();
				var c2 = db.Customers.Take(2).ToList();
				var skip = 3;
				var c3 = db.Customers.Take(skip).ToList();
				skip = 4;
				var c4 = db.Customers.Take(skip).ToList();

				var queryCache = GetQueryPlanCache();
				Assert.That(c1.Count, Is.EqualTo(1));
				Assert.That(c2.Count, Is.EqualTo(2));
				Assert.That(c3.Count, Is.EqualTo(3));
				Assert.That(c4.Count, Is.EqualTo(4));
				Assert.That(queryCache.Count, Is.EqualTo(supportsVariableLimit ? 1 : 4));
			}
		}

		[Test]
		public void TrimFunctionShouldNotBeCached()
		{
			ClearQueryPlanCache();

			db.Customers.Select(o => new {CustomerId = o.CustomerId.Trim('-')}).First();
			db.Customers.Select(o => new {CustomerId = o.CustomerId.Trim('+')}).First();

			var queryCache = GetQueryPlanCache();
			Assert.That(queryCache.Count, Is.EqualTo(0));
		}

		[Test]
		public void SubstringFunctionShouldBeCached()
		{
			ClearQueryPlanCache();

			var queryCache = GetQueryPlanCache();
			var c1 = db.Customers.Select(o => new {Name = o.ContactName.Substring(1)}).First();
			var c2 = db.Customers.Select(o => new {Name = o.ContactName.Substring(2)}).First();

			Assert.That(c1.Name, Is.Not.EqualTo(c2.Name));
			Assert.That(queryCache.Count, Is.EqualTo(1));

			ClearQueryPlanCache();
			c1 = db.Customers.Select(o => new { Name = o.ContactName.Substring(1, 2) }).First();
			c2 = db.Customers.Select(o => new { Name = o.ContactName.Substring(2, 1) }).First();

			Assert.That(c1.Name, Is.Not.EqualTo(c2.Name));
			Assert.That(queryCache.Count, Is.EqualTo(1));
		}
	}
}
