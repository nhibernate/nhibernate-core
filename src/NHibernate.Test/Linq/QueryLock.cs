using System.Linq;
using System.Transactions;
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
						var result2 = (from e in s2.Query<Customer>()
									   where e.CustomerId == customerId
									   select e).SetLockMode(LockMode.UpgradeNoWait)
												.Timeout(5).ToList();
						Assert.IsNotNull(result2);
					});
			}
		}
	}
}


