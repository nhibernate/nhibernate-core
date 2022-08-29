using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine;
using NHibernate.Linq;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class WithOptionsTests
	{
		[Test]
		public void AppliesOptionsToQuery()
		{
			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Settings.Returns(new Settings());

			var query = Substitute.For<IQuery>();

			query.List().Returns(new List<Order>());
			query.List<Order>().Returns(new List<Order>());

			session.CreateQuery(Arg.Any<IQueryExpression>()).Returns(query);

			var queryable = new NhQueryable<Order>(session)
				.WithOptions(
					x => x
						.SetCacheable(true)
						.SetCacheMode(CacheMode.Normal)
						.SetCacheRegion("testregion")
						.SetTimeout(10)
				);

			var t = queryable.ToList();

			query.Received(1).SetCacheable(true);
			query.Received(1).SetCacheMode(CacheMode.Normal);
			query.Received(1).SetCacheRegion("testregion");
			query.Received(1).SetTimeout(10);

			// Prevent garbage collection of session substitute before end of test, since the Linq query provider
			// only has a weak reference on it.
			Assert.That(session, Is.Not.Null);
		}

		[Test]
		public void DoNotContaminateQueryWithOptions()
		{
			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Settings.Returns(new Settings());

			var query = Substitute.For<IQuery>();

			query.List().Returns(new List<Order>());
			query.List<Order>().Returns(new List<Order>());

			session.CreateQuery(Arg.Any<IQueryExpression>()).Returns(query);

			var queryable = new NhQueryable<Order>(session);

			var o1 = queryable
				.WithOptions(
					x => x
						.SetCacheable(true)
						.SetCacheMode(CacheMode.Normal)
						.SetCacheRegion("testregion")
						.SetTimeout(10)
				).ToList();

			var o2 = queryable.ToList();

			query.Received(1).SetCacheable(true);
			query.Received(1).SetCacheMode(CacheMode.Normal);
			query.Received(1).SetCacheRegion("testregion");
			query.Received(1).SetTimeout(10);

			// Prevent garbage collection of session substitute before end of test, since the Linq query provider
			// only has a weak reference on it.
			Assert.That(session, Is.Not.Null);
		}

		[Test]
		public async Task AppliesOptionsToQueryAsync()
		{
			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Settings.Returns(new Settings());

			var query = Substitute.For<IQuery>();

			query.ListAsync().Returns(Task.FromResult((IList) new List<Order>()));

			session.CreateQuery(Arg.Any<IQueryExpression>()).Returns(query);

			var queryable = new NhQueryable<Order>(session)
				.WithOptions(
					x => x
						.SetCacheable(true)
						.SetCacheMode(CacheMode.Normal)
						.SetCacheRegion("testregion")
						.SetTimeout(10)
				);

			var t = await (queryable.ToListAsync());

			query.Received(1).SetCacheable(true);
			query.Received(1).SetCacheMode(CacheMode.Normal);
			query.Received(1).SetCacheRegion("testregion");
			query.Received(1).SetTimeout(10);

			// Prevent garbage collection of session substitute before end of test, since the Linq query provider
			// only has a weak reference on it.
			Assert.That(session, Is.Not.Null);
		}

		[Test]
		public async Task DoNotContaminateQueryWithOptionsAsync()
		{
			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Settings.Returns(new Settings());

			var query = Substitute.For<IQuery>();

			query.ListAsync().Returns(Task.FromResult((IList) new List<Order>()));

			session.CreateQuery(Arg.Any<IQueryExpression>()).Returns(query);

			var queryable = new NhQueryable<Order>(session);

			var o1 = await queryable
				.WithOptions(
					x => x
						.SetCacheable(true)
						.SetCacheMode(CacheMode.Normal)
						.SetCacheRegion("testregion")
						.SetTimeout(10)
				).ToListAsync();

			var o2 = await queryable
				.ToListAsync();

			query.Received(1).SetCacheable(true);
			query.Received(1).SetCacheMode(CacheMode.Normal);
			query.Received(1).SetCacheRegion("testregion");
			query.Received(1).SetTimeout(10);

			// Prevent garbage collection of session substitute before end of test, since the Linq query provider
			// only has a weak reference on it.
			Assert.That(session, Is.Not.Null);
		}
	}
}
