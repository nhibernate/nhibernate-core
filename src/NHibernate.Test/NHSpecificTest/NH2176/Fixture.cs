using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Dialect;
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
			//configuration.SetProperty(Cfg.Environment.TransactionStrategy, "NHibernate.Test.NHSpecificTest.NH2176.CustomAdoNetTransactionFactory, NHibernate.Test");
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var steve = new Person { Name = "Steve" };
				var peter = new Person { Name = "Peter" };
				var simon = new Person { Name = "Simon" };
				var paul = new Person { Name = "Paul" };
				var john = new Person { Name = "John" };
				var eric = new Person { Name = "Eric" };

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

					// The exception is caused by a race condition between two threads.
					// This can be demonstrated by uncommenting the following line which
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

		private readonly ConcurrentDictionary<DbConnection, System.Transactions.Transaction> _sessionsTransaction =
			new ConcurrentDictionary<DbConnection, System.Transactions.Transaction>();

		public void Configure(IDictionary props) { }

		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void EnlistInDistributedTransactionIfNeeded(ISessionImplementor session)
		{
			// No session enlistment. This disables automatic flushes before ambient transaction
			// commits. Explicit Flush calls required.
			// Still make sure the session connection is enlisted, in case it was acquired before 
			// transaction scope start.
			// Will not support nested transaction scope. (Will throw, while current NHibernate
			// just stay in previous scope.)
			// Will cause an "earlier than required" connection acquisition.
			// It is required to enlist with null when the scope is ended, otherwise using
			// the transaction without a new scope will fail by attempting to use it inside
			// the completed scope.
			// If an explicit transaction is ongoing, we must not enlist. We should not enlist
			// either if the connection was supplied by user (let him handle that in such case),
			// but there are currently no ways to know this from here.
			if (!session.ConnectionManager.Transaction.IsActive)
			{
				// Enlist is called terribly frequently, and in some circumstances, it will
				// not support to be called with the same value. So track what was the previous
				// call and do not call it again if unneeded.
				// (And Sql/OleDb/Odbc/Oracle manage/PostgreSql/MySql/Firebird/SQLite connections
				// support multiple calls with the same ongoing transaction, but some others may not.)
				var current = GetCurrentTransaction();
				var connection = session.Connection;
				System.Transactions.Transaction previous;
				if (!_sessionsTransaction.TryGetValue(connection, out previous) || previous != current)
				{
					_sessionsTransaction.AddOrUpdate(connection, current, (s, t) => current);
					if (current == null &&
						// This will need an ad-hoc property on Dialect base class instead.
						(session.Factory.Dialect is SQLiteDialect || session.Factory.Dialect is MsSqlCeDialect))
					{
						// Some connections does not support enlisting with null
						// Let them with their previous transaction if any, the application
						// will fail if the connection was left with a completed transaction due to this.
						return;
					}
					session.Connection.EnlistTransaction(current);
				}
			}
		}

		public bool IsInDistributedActiveTransaction(ISessionImplementor session)
		{
			// Avoid agressive connection release while a transaction is ongoing. Allow
			// auto-flushes (flushes before queries on dirtied entities).
			return GetCurrentTransaction() != null;
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

		private System.Transactions.Transaction GetCurrentTransaction()
		{
			try
			{
				return System.Transactions.Transaction.Current;
			}
			catch (InvalidOperationException)
			{
				// This damn thing may yield an invalid operation exception (instead of null
				// or of a completed transaction) if we are between scope.Complete() and
				// scope.Dispose(). This happen when having completed the scope then disposing
				// the session and only after that disposing the scope.
				// Instead of testing System.Transactions.Transaction.Current here, storing in
				// connection manager the ambient transaction associated to the connection
				// (and updating it when enlisting) then checking that stored transaction would
				// reduce testes on Transaction.Current.
				return null;
			}
		}
	}
}