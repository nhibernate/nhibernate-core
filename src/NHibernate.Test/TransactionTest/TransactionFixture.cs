using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.TransactionTest
{
	[TestFixture]
	public class TransactionFixture : TransactionFixtureBase
	{
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
		public void CommitAfterDisposeThrowsException()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				Assert.Throws<ObjectDisposedException>(() => t.Commit());
			}
		}

		[Test]
		public void RollbackAfterDisposeThrowsException()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();
				t.Dispose();
				Assert.Throws<ObjectDisposedException>(() => t.Rollback());
			}
		}

		[Test]
		public void EnlistAfterDisposeDoesNotThrowException()
		{
			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();

				using (var cmd = s.Connection.CreateCommand())
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
				using (s.BeginTransaction())
				{
				}

				s.CreateQuery("from Person").List();

				using (ITransaction t = s.BeginTransaction())
				{
					t.Commit();
				}

				s.CreateQuery("from Person").List();

				using (ITransaction t = s.BeginTransaction())
				{
					t.Rollback();
				}

				s.CreateQuery("from Person").List();
			}
		}

		[Test]
		public void WasCommittedOrRolledBack()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					Assert.AreSame(t, s.GetCurrentTransaction());
					Assert.IsFalse(s.GetCurrentTransaction().WasCommitted);
					Assert.IsFalse(s.GetCurrentTransaction().WasRolledBack);
					t.Commit();

					// ISession.GetCurrentTransaction() returns null if the transaction is completed.
					Assert.IsNull(s.GetCurrentTransaction());

					Assert.IsTrue(t.WasCommitted);
					Assert.IsFalse(t.WasRolledBack);
					Assert.IsFalse(t.IsActive);
				}

				using (ITransaction t = s.BeginTransaction())
				{
					t.Rollback();

					// ISession.GetCurrentTransaction() returns null if the transaction is completed.
					Assert.IsNull(s.GetCurrentTransaction());

					Assert.IsTrue(t.WasRolledBack);
					Assert.IsFalse(t.WasCommitted);

					Assert.IsFalse(t.IsActive);
				}
			}
		}

		[Test]
		public void FlushFromTransactionAppliesToSharingSession()
		{
			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var s1 = builder.Interceptor(new TestInterceptor(1, flushOrder)).OpenSession())
				using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
				using (var s3 = s1.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder))
								  .OpenSession())
				using (var t = s.BeginTransaction())
				{
					var p1 = new Person();
					var p2 = new Person();
					var p3 = new Person();
					var p4 = new Person();
					s1.Save(p1);
					s2.Save(p2);
					s3.Save(p3);
					s.Save(p4);
					t.Commit();
				}
			}

			Assert.That(flushOrder, Is.EqualTo(new[] { 0, 1, 2, 3 }));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(s.Query<Person>().Count(), Is.EqualTo(4));
				t.Commit();
			}
		}

		[Test]
		public void FlushFromTransactionAppliesToSharingStatelessSession()
		{
			using (var s = OpenSession())
			{
				var builder = s.StatelessSessionWithOptions().Connection();

				using (var s1 = builder.OpenStatelessSession())
				using (var s2 = builder.OpenStatelessSession())
				using (var t = s.BeginTransaction())
				{
					var p1 = new Person();
					var p2 = new Person();
					var p3 = new Person();
					s1.Insert(p1);
					s2.Insert(p2);
					s.Save(p3);
					t.Commit();
				}
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(s.Query<Person>().Count(), Is.EqualTo(3));
				t.Commit();
			}
		}

		// Taken and adjusted from NH1632 When_commiting_items_in_DTC_transaction_will_add_items_to_2nd_level_cache
		[Test]
		public void WhenCommittingItemsWillAddThemTo2ndLevelCache()
		{
			int id;
			const string notNullData = "test";
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var person = new CacheablePerson { NotNullData = notNullData };
				s.Save(person);
				id = person.Id;

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var person = s.Load<CacheablePerson>(id);
				Assert.That(person.NotNullData, Is.EqualTo(notNullData));
				t.Commit();
			}

			// Closing the connection to ensure we can't actually use it.
			var connection = Sfi.ConnectionProvider.GetConnection();
			Sfi.ConnectionProvider.CloseConnection(connection);

			// The session is supposed to succeed because the second level cache should have the
			// entity to load, allowing the session to not use the connection at all.
			// Will fail if a transaction manager tries to enlist user supplied connection. Do
			// not add a transaction scope below.
			using (var s = Sfi.WithOptions().Connection(connection).OpenSession())
			{
				CacheablePerson person = null;
				Assert.DoesNotThrow(
					() => person = s.Load<CacheablePerson>(id),
					"Failed loading entity from second level cache.");
				Assert.That(person.NotNullData, Is.EqualTo(notNullData));
			}
		}

		[Test]
		public void CanCommitFromSessionTransaction()
		{
			int id;
			using (var s = OpenSession())
			{
				Assert.That(s.GetCurrentTransaction(), Is.Null);
				using (s.BeginTransaction())
				{
					var person = new Person();
					s.Save(person);
					id = person.Id;

					s.GetCurrentTransaction().Commit();
				}
				Assert.That(s.GetCurrentTransaction(), Is.Null);
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var person = s.Get<Person>(id);
				Assert.That(person, Is.Not.Null);
				t.Commit();
			}
		}

		[Test]
		public void CanRollbackFromSessionTransaction()
		{
			int id;
			using (var s = OpenSession())
			{
				Assert.That(s.GetCurrentTransaction(), Is.Null);
				using (s.BeginTransaction())
				{
					var person = new Person();
					s.Save(person);
					id = person.Id;

					s.GetCurrentTransaction().Rollback();

					// Need to check before leaving the current using, otherwise the rollback could be the result of the
					// disposing.
					using (var s2 = OpenSession())
					using (var t2 = s2.BeginTransaction())
					{
						person = s2.Get<Person>(id);
						Assert.That(person, Is.Null);
						t2.Commit();
					}
				}
				Assert.That(s.GetCurrentTransaction(), Is.Null);
			}
		}

		[Test, Obsolete]
		public void CanCommitFromSessionObsoleteTransaction()
		{
			int id;
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var person = new Person();
				s.Save(person);
				id = person.Id;

				s.Transaction.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var person = s.Get<Person>(id);
				Assert.That(person, Is.Not.Null);
				t.Commit();
			}
		}

		[Test, Obsolete]
		public void CanRollbackFromSessionObsoleteTransaction()
		{
			int id;
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var person = new Person();
				s.Save(person);
				id = person.Id;

				s.Transaction.Rollback();

				// Need to check before leaving the current using, otherwise the rollback could be the result of the
				// disposing.
				using (var s2 = OpenSession())
				using (var t2 = s2.BeginTransaction())
				{
					person = s2.Get<Person>(id);
					Assert.That(person, Is.Null);
					t2.Commit();
				}
			}
		}
	}
}
