using System.Linq;
using System.Transactions;
using NHibernate.Dialect;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NUnit.Framework;


namespace NHibernate.Test.Linq
{
	public class QueryLock : LinqTestCase
	{
		[Test]
		public void CanSetLockLinqQueries()
		{
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers
				              select e).SetLockMode(LockMode.Upgrade).ToList();

				Assert.That(result, Has.Count.EqualTo(91));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));
				AssertSeparateTransactionIsLockedOut(result[0].CustomerId);
			}
		}


		[Test]
		public void CanSetLockOnLinqPagingQuery()
		{
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers
				              select e).Skip(5).Take(5).SetLockMode(LockMode.Upgrade).ToList();

				Assert.That(result, Has.Count.EqualTo(5));
				Assert.That(session.GetCurrentLockMode(result[0]), Is.EqualTo(LockMode.Upgrade));
				AssertSeparateTransactionIsLockedOut(result[0].CustomerId);
			}
		}

		[Test]
		public void CanLockBeforeSkipOnLinqOrderedPageQuery()
		{
			using (session.BeginTransaction())
			{
				var result = (from e in db.Customers
				              orderby e.CompanyName
				              select e)
				             .SetLockMode(LockMode.Upgrade).Skip(5).Take(5).ToList();

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
							).SetLockMode(LockMode.UpgradeNoWait)
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
			return (from e in dbCustomers select e).SetLockMode(lockMode).WithOptions(o => o.SetTimeout(5));
		}
	}
}


