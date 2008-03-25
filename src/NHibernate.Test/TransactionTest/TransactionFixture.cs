using System;
using System.Collections;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.TransactionTest
{
	[TestFixture]
	public class TransactionFixture : TestCase
	{
		protected override IList Mappings
		{
			// The mapping is only actually needed in one test
			get { return new string[] {"Simple.hbm.xml"}; }
		}

		[Test]
		public void SecondTransactionShouldntBeCommitted()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction t1 = session.BeginTransaction())
				{
					t1.Commit();
				}

				using (ITransaction t2 = session.BeginTransaction())
				{
					Assert.IsFalse(t2.WasCommitted);
					Assert.IsFalse(t2.WasRolledBack);
				}
			}
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void CommitAfterDisposeThrowsException()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				t.Commit();
			}
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void RollbackAfterDisposeThrowsException()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				t.Rollback();
			}
		}

		[Test]
		public void EnlistAfterDisposeDoesNotThrowException()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				using (IDbCommand cmd = s.Connection.CreateCommand())
				{
					t.Dispose();
					t.Enlist(cmd);
				}
			}
		}

		[Test]
		public void CommandAfterTransactionShouldWork()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
				}

				s.CreateQuery("from Simple").List();

				using (ITransaction t = s.BeginTransaction())
				{
					t.Commit();
				}

				s.CreateQuery("from Simple").List();

				using (ITransaction t = s.BeginTransaction())
				{
					t.Rollback();
				}

				s.CreateQuery("from Simple").List();
			}
		}

		[Test]
		public void WasCommittedOrRolledBack()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Assert.AreSame(t, s.Transaction);
					Assert.IsFalse(s.Transaction.WasCommitted);
					Assert.IsFalse(s.Transaction.WasRolledBack);
					t.Commit();

					// ISession.Transaction returns a new transaction
					// if the previous one completed!
					Assert.IsNotNull(s.Transaction);
					Assert.IsFalse(t == s.Transaction);

					Assert.IsTrue(t.WasCommitted);
					Assert.IsFalse(t.WasRolledBack);
					Assert.IsFalse(s.Transaction.WasCommitted);
					Assert.IsFalse(s.Transaction.WasRolledBack);
					Assert.IsFalse(t.IsActive);
					Assert.IsFalse(s.Transaction.IsActive);
				}

				using (ITransaction t = s.BeginTransaction())
				{
					t.Rollback();

					// ISession.Transaction returns a new transaction
					// if the previous one completed!
					Assert.IsNotNull(s.Transaction);
					Assert.IsFalse(t == s.Transaction);

					Assert.IsTrue(t.WasRolledBack);
					Assert.IsFalse(t.WasCommitted);

					Assert.IsFalse(s.Transaction.WasCommitted);
					Assert.IsFalse(s.Transaction.WasRolledBack);

					Assert.IsFalse(t.IsActive);
					Assert.IsFalse(s.Transaction.IsActive);
				}
			}
		}
	}
}