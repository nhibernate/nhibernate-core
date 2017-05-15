using System;
using System.Collections;
using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Transaction;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2176
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.TransactionStrategy, "NHibernate.Test.NHSpecificTest.NH2176.CustomAdoNetTransactionFactory, NHibernate.Test");
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var steve = new Person {Name = "Steve"};
				var peter = new Person {Name = "Peter"};
				var simon = new Person {Name = "Simon"};
				var paul = new Person {Name = "Paul"};
				var john = new Person {Name = "John"};
				var eric = new Person {Name = "Eric"};

				s.Save(steve);
				s.Save(peter);
				s.Save(simon);
				s.Save(paul);
				s.Save(john);
				s.Save(eric);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		// Whilst this bug seems specific to Oracle I think it is valid to run the
		// test against all database types.
		[Test]
		public void MultipleConsecutiveTransactionScopesCanBeUsedInsideASingleSession()
		{
			using (var s = OpenSession())
			{
				// usually fails after just a few loops in oracle
				// this can be run for 10000 loops in sql server without problem
				for (var i = 0; i < 100; ++i)
				{
					Console.WriteLine(i.ToString());

					using (var scope = new TransactionScope())
					{
						var criteria = s.CreateCriteria<Person>();
						var people = criteria.List<Person>();

						Assert.That(people.Count, Is.EqualTo(6));

						scope.Complete();
					}

					// The exeption is caused by a race condition between two threads.
					// This can be demonstracted by uncommenting the following line which
					// causes the test to run without an exception.
					//System.Threading.Thread.Sleep(1000);
				}
			}
		}
	}

	// Unfortunately, cannot derive and override NHibernate impl, methods are not virtual.
	public class CustomAdoNetTransactionFactory : ITransactionFactory
	{
		private readonly AdoNetTransactionFactory _adoNetTransactionFactory =
			new AdoNetTransactionFactory();

		public void Configure(IDictionary props) { }

		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void EnlistInDistributedTransactionIfNeeded(ISessionImplementor session)
		{
			// No enlistment. This disables automatic flushes before ambient transaction
			// commits. Explicit Flush calls required.
		}

		public bool IsInDistributedActiveTransaction(ISessionImplementor session)
		{
			// Avoid agressive connection release while a transaction is ongoing. Allow
			// auto-flushes (flushes before queries on dirtied entities).
			return System.Transactions.Transaction.Current != null;
		}

		public void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work,
			bool transacted)
		{
			using (var tx = new TransactionScope(TransactionScopeOption.Suppress))
			{
				// instead of duplicating the logic, we suppress the DTC transaction
				// and create our own transaction instead
				_adoNetTransactionFactory.ExecuteWorkInIsolation(session, work,
					transacted);
				tx.Complete();
			}
		}
	}
}