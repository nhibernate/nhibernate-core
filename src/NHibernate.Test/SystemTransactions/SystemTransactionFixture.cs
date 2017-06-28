using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Test.TransactionTest;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	[TestFixture]
	public class SystemTransactionFixture : SystemTransactionFixtureBase
	{
		protected virtual bool AutoJoinTransaction => true;

		[Test]
		public void WillNotCrashOnPrepareFailure()
		{
			var tx = new TransactionScope();
			var disposeCalled = false;
			try
			{
				using (var s = OpenSession())
				{
					s.Save(new Person { NotNullData = null }); // Cause a SQL not null constraint violation.
				}

				tx.Complete();
				disposeCalled = true;
				Assert.Throws<TransactionAbortedException>(tx.Dispose, "Scope disposal has not rollback and throw.");
			}
			finally
			{
				if (!disposeCalled)
				{
					try
					{
						tx.Dispose();
					}
					catch
					{
						// Ignore, if disposed has not been called, another exception has occurred in the try and
						// we should avoid overriding it by the disposal failure.
					}
				}
			}
		}

		[Test]
		public void CanRollbackTransactionFromScope([Values(false, true)] bool explicitFlush)
		{
			using (new TransactionScope())
			using (var s = OpenSession())
			{
				s.Save(new Person());

				if (explicitFlush)
					s.Flush();
				// No Complete call for triggering rollback.
			}

			AssertNoPersons();
		}

		[Test]
		[Description("rollback inside nh-session-scope should not commit save and the transaction should be aborted.")]
		public void TransactionInsertWithRollBackFromScope([Values(false, true)] bool explicitFlush)
		{
			using (new TransactionScope())
			{
				using (var s = OpenSession())
				{
					var person = new Person();
					s.Save(person);

					if (explicitFlush)
						s.Flush();
				}
				// No Complete call for triggering rollback.
			}
			AssertNoPersons();
		}

		[Test]
		[Description(@"Two session in two txscope
 (without an explicit NH transaction)
 and with a rollback in the second and a rollback outside nh-session-scope.")]
		public void TransactionInsertLoadWithRollBackFromScope([Values(false, true)] bool explicitFlush)
		{
			object savedId;
			var createdAt = DateTime.Today;
			using (var txscope = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					var person = new Person { CreatedAt = createdAt };
					savedId = s.Save(person);

					if (explicitFlush)
						s.Flush();
				}
				txscope.Complete();
			}

			using (new TransactionScope())
			{
				using (var s = OpenSession())
				{
					var person = s.Get<Person>(savedId);
					person.CreatedAt = createdAt.AddMonths(-1);

					if (explicitFlush)
						s.Flush();
				}

				// No Complete call for triggering rollback.
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(createdAt, s.Get<Person>(savedId).CreatedAt, "Entity update was not rollback-ed.");
			}
		}

		[Test]
		public void CanDeleteItem([Values(false, true)] bool explicitFlush)
		{
			object id;
			using (var tx = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					id = s.Save(new Person());

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(1, s.Query<Person>().Count(), "Entity not found in database.");
			}

			using (var tx = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					s.Delete(s.Get<Person>(id));

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}
			}

			AssertNoPersons();
		}

		[Test]
		public void CanUseSessionWithManyScopes([Values(false, true)] bool explicitFlush)
		{
			using (var s = WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
			{
				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					// Acquire the connection
					var count = s.Query<Person>().Count();
					Assert.That(count, Is.EqualTo(0), "Unexpected initial entity count.");
					tx.Complete();
				}

				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					s.Save(new Person());

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}

				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var count = s.Query<Person>().Count();
					Assert.That(count, Is.EqualTo(1), "Unexpected entity count after committed insert.");
					tx.Complete();
				}

				using (new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					s.Save(new Person());

					if (explicitFlush)
						s.Flush();

					// No complete for rollback-ing.
				}

				// Do not reuse the session after a rollback, its state does not allow it.
				// http://nhibernate.info/doc/nhibernate-reference/manipulatingdata.html#manipulatingdata-endingsession-commit
			}

			using (var s = OpenSession())
			{
				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var count = s.Query<Person>().Count();
					Assert.That(count, Is.EqualTo(1), "Unexpected entity count after rollback-ed insert.");
					tx.Complete();
				}
			}
		}

		[Test]
		public void CanUseSessionOutsideOfScopeAfterScope([Values(false, true)] bool explicitFlush)
		{
			using (var s = WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
			{
				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					s.Save(new Person());

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}
				var count = 0;
				Assert.DoesNotThrow(() => count = s.Query<Person>().Count(), "Failed using the session after scope.");
				if (count != 1)
					// We are not testing that here, so just issue a warning. Do not use DodgeTransactionCompletionDelayIfRequired
					// before previous assert. We want to ascertain the session is usable in any cases.
					Assert.Warn("Unexpected entity count: {0} instead of {1}. The transaction seems to have a delayed commit.", count, 1);
			}
		}

		[Test(Description = "Do not fail, but warn in case a delayed after scope disposal commit is made.")]
		public void DelayedTransactionCompletion([Values(false, true)] bool explicitFlush)
		{
			for (var i = 1; i <= 10; i++)
			{
				// Isolation level must be read committed on the control session: reading twice while expecting some data insert
				// in between due to a late commit. Repeatable read would block and read uncommitted would see the uncommitted data.
				using (var controlSession = OpenSession())
				using (controlSession.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
				{
					// We want to have the control session as ready to query as possible, thus beginning its
					// transaction early for acquiring the connection, even if we will not use it before 
					// below scope completion.

					using (var tx = new TransactionScope())
					{
						using (var s = OpenSession())
						{
							s.Save(new Person());

							if (explicitFlush)
								s.Flush();
						}
						tx.Complete();
					}

					var count = controlSession.Query<Person>().Count();
					if (count != i)
					{
						Thread.Sleep(100);
						var countSecondTry = controlSession.Query<Person>().Count();
						Assert.Warn($"Unexpected entity count: {count} instead of {i}. " +
									"This may mean current data provider has a delayed commit, occurring after scope disposal. " +
									$"After waiting, count is now {countSecondTry}. ");
						break;
					}
				}
			}
		}

		[Test]
		public void FlushFromTransactionAppliesToDisposedSharingSession()
		{
			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var t = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var p1 = new Person();
					var p2 = new Person();
					var p3 = new Person();
					var p4 = new Person();

					using (var s1 = builder.Interceptor(new TestInterceptor(1, flushOrder)).OpenSession())
					{
						if (!AutoJoinTransaction)
							s1.JoinTransaction();
						s1.Save(p1);
					}
					using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
					{
						if (!AutoJoinTransaction)
							s2.JoinTransaction();
						s2.Save(p2);
						using (var s3 = s2.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder))
							.OpenSession())
						{
							if (!AutoJoinTransaction)
								s3.JoinTransaction();
							s3.Save(p3);
						}
					}
					s.Save(p4);
					t.Complete();
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
		public void FlushFromTransactionAppliesToSharingSession()
		{
			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var s1 = builder.Interceptor(new TestInterceptor(1, flushOrder)).OpenSession())
				using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
				using (var s3 = s2.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder)).OpenSession())
				using (var t = new TransactionScope())
				{
					if (!AutoJoinTransaction)
					{
						s.JoinTransaction();
						s1.JoinTransaction();
						s2.JoinTransaction();
						s3.JoinTransaction();
					}
					var p1 = new Person();
					var p2 = new Person();
					var p3 = new Person();
					var p4 = new Person();
					s1.Save(p1);
					s2.Save(p2);
					s3.Save(p3);
					s.Save(p4);
					t.Complete();
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
		public void DoNotDeadlockOnAfterTransactionWait()
		{
			var interceptor = new AfterTransactionWaitingInterceptor();
			using (var s = WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = new TransactionScope())
			{
				if (!AutoJoinTransaction)
					s.JoinTransaction();
				s.Save(new Person());

				s.Flush();
				tx.Complete();
			}
			Assert.That(interceptor.Exception, Is.Null);
		}

		[Test]
		public void AdditionalJoinDoesNotThrow()
		{
			using (new TransactionScope())
			using (var s = OpenSession())
			{
				Assert.DoesNotThrow(() => s.JoinTransaction());
			}
		}

		protected override ISession OpenSession()
		{
			if (AutoJoinTransaction)
				return base.OpenSession();

			var session = Sfi.WithOptions().AutoJoinTransaction(false).OpenSession();
			if (System.Transactions.Transaction.Current != null)
				session.JoinTransaction();
			return session;
		}

		protected ISessionBuilder WithOptions()
		{
			return Sfi.WithOptions().AutoJoinTransaction(AutoJoinTransaction);
		}
	}

	[TestFixture]
	public class SystemTransactionWithoutConnectionAutoEnlistmentFixture : SystemTransactionFixture
	{
		protected override void Configure(Configuration configuration)
		{
			var connectionString = configuration.GetProperty(Cfg.Environment.ConnectionString);
			var autoEnlistmentKeyword = "Enlist";
			var autoEnlistmentKeywordPattern = autoEnlistmentKeyword;
			if (configuration.GetDerivedProperties().TryGetValue(Cfg.Environment.ConnectionDriver, out var driver) &&
				typeof(MySqlDataDriver).IsAssignableFrom(ReflectHelper.ClassForName(driver)))
			{
				autoEnlistmentKeyword = "AutoEnlist";
				autoEnlistmentKeywordPattern = "Auto ?Enlist";
			}
			// Purge any previous enlist
			connectionString = Regex.Replace(
				connectionString, $"[^;\"a-zA-Z]*{autoEnlistmentKeywordPattern}=[^;\"]*", string.Empty,
				RegexOptions.IgnoreCase | RegexOptions.Multiline);
			connectionString += $";{autoEnlistmentKeyword}=false;";
			configuration.SetProperty(Cfg.Environment.ConnectionString, connectionString);
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> base.AppliesTo(factory) && factory.ConnectionProvider.Driver.SupportsEnlistmentWhenAutoEnlistmentIsDisabled;
	}

	[TestFixture]
	public class SystemTransactionWithoutAutoJoinTransaction : SystemTransactionWithoutConnectionAutoEnlistmentFixture
	{
		protected override bool AutoJoinTransaction => false;

		[Test]
		public void SessionIsNotEnlisted()
		{
			using (new TransactionScope())
			// Dodge the OpenSession override which call JoinTransaction by calling WithOptions().
			using (var s = WithOptions().OpenSession())
			{
				Assert.That(s.GetSessionImplementation().TransactionContext, Is.Null);
			}
		}
	}
}