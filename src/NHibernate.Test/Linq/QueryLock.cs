using System;
using System.Linq;
using System.Transactions;
using NHibernate.Dialect;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryLock : LinqTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsSelectForUpdate;
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return !(factory.ConnectionProvider.Driver is OdbcDriver);
		}

		[Test]
		public void CanSetLockLinqQueriesOuter()
		{
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers
							  select e).WithLock(LockMode.Upgrade).ToList();

				Assert.That(result, Has.Count.EqualTo(91));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));
				AssertSeparateTransactionIsLockedOut(result[0].CustomerId);
			}
		}

		[Test]
		public void CanSetLockLinqQueries()
		{
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers.WithLock(LockMode.Upgrade)
							  select e).ToList();

				Assert.That(result, Has.Count.EqualTo(91));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));
				AssertSeparateTransactionIsLockedOut(result[0].CustomerId);
			}
		}

		[Test]
		public void CanSetLockOnJoinHql()
		{
			using (session.BeginTransaction())
			{
				session
					.CreateQuery("select o from Customer c join c.Orders o")
					.SetLockMode("o", LockMode.Upgrade)
					.List();
			}
		}

		[Test]
		public void CanSetLockOnJoin()
		{
			using (session.BeginTransaction())
			{
				var result = (from c in db.Customers
							  from o in c.Orders.WithLock(LockMode.Upgrade)
							  select o).ToList();

				Assert.That(result, Has.Count.EqualTo(830));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));
			}
		}

		[Test]
		public void CanSetLockOnJoinOuter()
		{
			using (session.BeginTransaction())
			{
				var result = (from c in db.Customers
							  from o in c.Orders
							  select o).WithLock(LockMode.Upgrade).ToList();

				Assert.That(result, Has.Count.EqualTo(830));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));
			}
		}

		[Test]
		public void CanSetLockOnJoinOuterNotSupported()
		{
			using (session.BeginTransaction())
			{
				var query = (
					from c in db.Customers
					from o in c.Orders
					select new { o, c }
				).WithLock(LockMode.Upgrade);

				Assert.Throws<NotSupportedException>(() => query.ToList());
			}
		}

		[Test]
		public void CanSetLockOnJoinOuter2Hql()
		{
			using (session.BeginTransaction())
			{
				session
					.CreateQuery("select o, c from Customer c join c.Orders o")
					.SetLockMode("o", LockMode.Upgrade)
					.SetLockMode("c", LockMode.Upgrade)
					.List();
			}
		}

		[Test]
		public void CanSetLockOnBothJoinAndMain()
		{
			using (session.BeginTransaction())
			{
				var result = (
					from c in db.Customers.WithLock(LockMode.Upgrade)
					from o in c.Orders.WithLock(LockMode.Upgrade)
					select new { o, c }
				).ToList();

				Assert.That(result, Has.Count.EqualTo(830));
				Assert.That(session.GetCurrentLockMode(result[0].o), Is.EqualTo(LockMode.Upgrade));
				Assert.That(session.GetCurrentLockMode(result[0].c), Is.EqualTo(LockMode.Upgrade));
			}
		}

		[Test]
		public void CanSetLockOnBothJoinAndMainComplex()
		{
			using (session.BeginTransaction())
			{
				var result = (
					from c in db.Customers.Where(x => true).WithLock(LockMode.Upgrade)
					from o in c.Orders.Where(x => true).WithLock(LockMode.Upgrade)
					select new { o, c }
				).ToList();

				Assert.That(result, Has.Count.EqualTo(830));
				Assert.That(session.GetCurrentLockMode(result[0].o), Is.EqualTo(LockMode.Upgrade));
				Assert.That(session.GetCurrentLockMode(result[0].c), Is.EqualTo(LockMode.Upgrade));
			}
		}

		[Test]
		public void CanSetLockOnLinqPagingQuery()
		{
			Assume.That(TestDialect.SupportsSelectForUpdateWithPaging, Is.True, "Dialect does not support locking in subqueries");
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers
							  select e).Skip(5).Take(5).WithLock(LockMode.Upgrade).ToList();

				Assert.That(result, Has.Count.EqualTo(5));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));

				AssertSeparateTransactionIsLockedOut(result[0].CustomerId);
			}
		}

		[Test]
		public void CanLockBeforeSkipOnLinqOrderedPageQuery()
		{
			Assume.That(TestDialect.SupportsSelectForUpdateWithPaging, Is.True, "Dialect does not support locking in subqueries");
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers
							  orderby e.CompanyName
							  select e)
							 .WithLock(LockMode.Upgrade).Skip(5).Take(5).ToList();

				Assert.That(result, Has.Count.EqualTo(5));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));

				AssertSeparateTransactionIsLockedOut(result[0].CustomerId);
			}
		}

		private void AssertSeparateTransactionIsLockedOut(string customerId)
		{
			using (new TransactionScope(TransactionScopeOption.Suppress))
			using (var s2 = OpenSession())
			using (s2.BeginTransaction())
			{
				// TODO: We should try to verify that the exception actually IS a locking failure and not something unrelated.
				Assert.Throws<GenericADOException>(
					() =>
					{
						var result2 = (
								from e in s2.Query<Customer>()
								where e.CustomerId == customerId
								select e
							).WithLock(LockMode.UpgradeNoWait)
							 .WithOptions(o => o.SetTimeout(5))
							 .ToList();
						Assert.That(result2, Is.Not.Null);
					},
					"Expected an exception to indicate locking failure due to already locked.");
			}
		}

		[Test]
		[Description("Verify that different lock modes are respected even if the query is otherwise exactly the same.")]
		public void CanChangeLockModeForQuery()
		{
			// Limit to a few dialects where we know the "nowait" keyword is used to make life easier.
			Assume.That(Dialect is MsSql2000Dialect || Dialect is Oracle8iDialect || Dialect is PostgreSQL81Dialect);

			using (session.BeginTransaction())
			{
				var result = BuildQueryableAllCustomers(db.Customers, LockMode.Upgrade).ToList();
				Assert.That(result, Has.Count.EqualTo(91));

				using (var logSpy = new SqlLogSpy())
				{
					// Only difference in query is the lockmode - make sure it gets picked up.
					var result2 = BuildQueryableAllCustomers(session.Query<Customer>(), LockMode.UpgradeNoWait)
						.ToList();
					Assert.That(result2, Has.Count.EqualTo(91));

					Assert.That(logSpy.GetWholeLog().ToLower(), Does.Contain("nowait"));
				}
			}
		}

		private static IQueryable<Customer> BuildQueryableAllCustomers(
			IQueryable<Customer> dbCustomers,
			LockMode lockMode)
		{
			return (from e in dbCustomers select e).WithLock(lockMode).WithOptions(o => o.SetTimeout(5));
		}
	}
}
