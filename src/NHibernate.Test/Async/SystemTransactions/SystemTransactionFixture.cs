﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Test.TransactionTest;
using NUnit.Framework;
using sysTran = System.Transactions;
using NHibernate.Linq;

namespace NHibernate.Test.SystemTransactions
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SystemTransactionFixtureAsync : SystemTransactionFixtureBase
	{
		protected override bool UseConnectionOnSystemTransactionPrepare => true;
		protected override bool AutoJoinTransaction => true;

		[Test]
		public async Task WillNotCrashOnPrepareFailureAsync()
		{
			IgnoreIfUnsupported(false);
			var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			var disposeCalled = false;
			try
			{
				using (var s = OpenSession())
				{
					await (s.SaveAsync(new Person { NotNullData = null })); // Cause a SQL not null constraint violation.
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

		[Theory]
		public async Task CanRollbackTransactionFromScopeAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			using (var s = OpenSession())
			{
				await (s.SaveAsync(new Person()));

				if (explicitFlush)
					await (s.FlushAsync());
				// No Complete call for triggering rollback.
			}

			AssertNoPersons();
		}

		[Theory]
		[Description("rollback inside nh-session-scope should not commit save and the transaction should be aborted.")]
		public async Task TransactionInsertWithRollBackFromScopeAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					var person = new Person();
					await (s.SaveAsync(person));

					if (explicitFlush)
						await (s.FlushAsync());
				}
				// No Complete call for triggering rollback.
			}
			AssertNoPersons();
		}

		[Theory]
		[Description(@"Two session in two txscope
 (without an explicit NH transaction)
 and with a rollback in the second and a rollback outside nh-session-scope.")]
		public async Task TransactionInsertLoadWithRollBackFromScopeAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			object savedId;
			var createdAt = DateTime.Today;
			using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					var person = new Person { CreatedAt = createdAt };
					savedId = await (s.SaveAsync(person));

					if (explicitFlush)
						await (s.FlushAsync());
				}
				txscope.Complete();
			}

			using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					var person = await (s.GetAsync<Person>(savedId));
					person.CreatedAt = createdAt.AddMonths(-1);

					if (explicitFlush)
						await (s.FlushAsync());
				}

				// No Complete call for triggering rollback.
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(createdAt, (await (s.GetAsync<Person>(savedId))).CreatedAt, "Entity update was not rollback-ed.");
			}
		}

		[Theory]
		public async Task CanDeleteItemAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			object id;
			using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					id = await (s.SaveAsync(new Person()));

					if (explicitFlush)
						await (s.FlushAsync());

					tx.Complete();
				}
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(1, await (s.Query<Person>().CountAsync()), "Entity not found in database.");
			}

			using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					await (s.DeleteAsync(await (s.GetAsync<Person>(id))));

					if (explicitFlush)
						await (s.FlushAsync());

					tx.Complete();
				}
			}

			AssertNoPersons();
		}

		[Theory]
		public async Task CanUseSessionWithManyScopesAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			// ODBC with SQL-Server always causes scopes to go distributed, which causes their transaction completion to run
			// asynchronously. But ODBC enlistment also check the previous transaction in a way that do not guard against it
			// being concurrently disposed of. See https://github.com/nhibernate/nhibernate-core/pull/1505 for more details.
			if (Sfi.ConnectionProvider.Driver is OdbcDriver)
				Assert.Ignore("ODBC sometimes fails on second scope by checking the previous transaction status, which may yield an object disposed exception");
			// SAP HANA & SQL Anywhere .Net providers always cause system transactions to be distributed, causing them to
			// complete on concurrent threads. This creates race conditions when chaining scopes, the subsequent scope usage
			// finding the connection still enlisted in the previous transaction, its complete being still not finished
			// on its own thread.
			if (Sfi.ConnectionProvider.Driver is HanaDriverBase || Sfi.ConnectionProvider.Driver is SapSQLAnywhere17Driver)
				Assert.Ignore("SAP HANA and SQL Anywhere scope handling causes concurrency issues preventing chaining scope usages.");

			using (var s = WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
			{
				using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					// Acquire the connection
					var count = await (s.Query<Person>().CountAsync());
					Assert.That(count, Is.EqualTo(0), "Unexpected initial entity count.");
					tx.Complete();
				}

				using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					await (s.SaveAsync(new Person()));

					if (explicitFlush)
						await (s.FlushAsync());

					tx.Complete();
				}

				using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var count = await (s.Query<Person>().CountAsync());
					Assert.That(count, Is.EqualTo(1), "Unexpected entity count after committed insert.");
					tx.Complete();
				}

				using (new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					await (s.SaveAsync(new Person()));

					if (explicitFlush)
						await (s.FlushAsync());

					// No complete for rollback-ing.
				}

				// Do not reuse the session after a rollback, its state does not allow it.
				// http://nhibernate.info/doc/nhibernate-reference/manipulatingdata.html#manipulatingdata-endingsession-commit
			}

			using (var s = OpenSession())
			{
				using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var count = await (s.Query<Person>().CountAsync());
					Assert.That(count, Is.EqualTo(1), "Unexpected entity count after rollback-ed insert.");
					tx.Complete();
				}
			}
		}

		[Theory]
		public async Task CanUseSessionOutsideOfScopeAfterScopeAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			// SAP SQL Anywhere .Net provider always causes system transactions to be distributed, causing them to
			// complete on concurrent threads. This creates race conditions when chaining session usage after a scope,
			// the subsequent usage finding the connection still enlisted in the previous transaction, its complete
			// being still not finished on its own thread.
			if (Sfi.ConnectionProvider.Driver is SapSQLAnywhere17Driver)
				Assert.Ignore("SAP SQL Anywhere scope handling causes concurrency issues preventing chaining session usages.");

			using (var s = WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
			{
				using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					await (s.SaveAsync(new Person()));

					if (explicitFlush)
						await (s.FlushAsync());

					tx.Complete();
				}
				var count = 0;
				Assert.DoesNotThrowAsync(async () => count = await (s.Query<Person>().CountAsync()), "Failed using the session after scope.");
				if (count != 1)
					// We are not testing that here, so just issue a warning. Do not use DodgeTransactionCompletionDelayIfRequired
					// before previous assert. We want to ascertain the session is usable in any cases.
					Assert.Warn("Unexpected entity count: {0} instead of {1}. The transaction seems to have a delayed commit.", count, 1);
			}
		}

		[Theory]
		[Description("Do not fail, but warn in case a delayed after scope disposal commit is made.")]
		public async Task DelayedTransactionCompletionAsync(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
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

					using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
					{
						using (var s = OpenSession())
						{
							await (s.SaveAsync(new Person()));

							if (explicitFlush)
								await (s.FlushAsync());
						}
						tx.Complete();
					}

					var count = await (controlSession.Query<Person>().CountAsync());
					if (count != i)
					{
						await (Task.Delay(100));
						var countSecondTry = await (controlSession.Query<Person>().CountAsync());
						Assert.Warn($"Unexpected entity count: {count} instead of {i}. " +
									"This may mean current data provider has a delayed commit, occurring after scope disposal. " +
									$"After waiting, count is now {countSecondTry}. ");
						break;
					}
				}
			}
		}

		[Test]
		public async Task FlushFromTransactionAppliesToDisposedSharingSessionAsync()
		{
			IgnoreIfUnsupported(false);

			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var t = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
						await (s1.SaveAsync(p1));
					}
					using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
					{
						if (!AutoJoinTransaction)
							s2.JoinTransaction();
						await (s2.SaveAsync(p2));
						using (var s3 = s2.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder))
							.OpenSession())
						{
							if (!AutoJoinTransaction)
								s3.JoinTransaction();
							await (s3.SaveAsync(p3));
						}
					}
					await (s.SaveAsync(p4));
					t.Complete();
				}
			}

			Assert.That(flushOrder, Is.EqualTo(new[] { 0, 1, 2, 3 }));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(await (s.Query<Person>().CountAsync()), Is.EqualTo(4));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task FlushFromTransactionAppliesToSharingSessionAsync()
		{
			IgnoreIfUnsupported(false);

			var flushOrder = new List<int>();
			using (var s = OpenSession(new TestInterceptor(0, flushOrder)))
			{
				var builder = s.SessionWithOptions().Connection();

				using (var s1 = builder.Interceptor(new TestInterceptor(1, flushOrder)).OpenSession())
				using (var s2 = builder.Interceptor(new TestInterceptor(2, flushOrder)).OpenSession())
				using (var s3 = s2.SessionWithOptions().Connection().Interceptor(new TestInterceptor(3, flushOrder)).OpenSession())
				using (var t = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
					await (s1.SaveAsync(p1));
					await (s2.SaveAsync(p2));
					await (s3.SaveAsync(p3));
					await (s.SaveAsync(p4));
					t.Complete();
				}
			}

			Assert.That(flushOrder, Is.EqualTo(new[] { 0, 1, 2, 3 }));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Assert.That(await (s.Query<Person>().CountAsync()), Is.EqualTo(4));
				await (t.CommitAsync());
			}
		}

		// Taken and adjusted from NH1632 When_commiting_items_in_DTC_transaction_will_add_items_to_2nd_level_cache
		[Test]
		public async Task WhenCommittingItemsAfterSessionDisposalWillAddThemTo2ndLevelCacheAsync()
		{
			int id;
			const string notNullData = "test";
			using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					var person = new CacheablePerson { NotNullData = notNullData };
					await (s.SaveAsync(person));
					id = person.Id;

					await (s.FlushAsync());
				}
				tx.Complete();
			}

			using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				using (var s = OpenSession())
				{
					var person = await (s.LoadAsync<CacheablePerson>(id));
					Assert.That(person.NotNullData, Is.EqualTo(notNullData));
				}
				tx.Complete();
			}

			// Closing the connection to ensure we can't actually use it.
			var connection = await (Sfi.ConnectionProvider.GetConnectionAsync(CancellationToken.None));
			Sfi.ConnectionProvider.CloseConnection(connection);

			// The session is supposed to succeed because the second level cache should have the
			// entity to load, allowing the session to not use the connection at all.
			// Will fail if a transaction manager tries to enlist user supplied connection. Do
			// not add a transaction scope below.
			using (var s = WithOptions().Connection(connection).OpenSession())
			{
				CacheablePerson person = null;
				Assert.DoesNotThrowAsync(async () => person = await (s.LoadAsync<CacheablePerson>(id)), "Failed loading entity from second level cache.");
				Assert.That(person.NotNullData, Is.EqualTo(notNullData));
			}
		}

		[Test]
		public async Task DoNotDeadlockOnAfterTransactionWaitAsync()
		{
			var interceptor = new AfterTransactionWaitingInterceptor();
			using (var s = WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				if (!AutoJoinTransaction)
					s.JoinTransaction();
				await (s.SaveAsync(new Person()));

				await (s.FlushAsync());
				tx.Complete();
			}
			Assert.That(interceptor.Exception, Is.Null);
		}

		[Test]
		public async Task EnforceConnectionUsageRulesOnTransactionCompletionAsync()
		{
			var interceptor = new TransactionCompleteUsingConnectionInterceptor();
			// Do not invert session and scope, it would cause an expected failure when
			// UseConnectionOnSystemTransactionEvents is false, due to the session being closed.
			using (var s = WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				if (!AutoJoinTransaction)
					s.JoinTransaction();
				await (s.SaveAsync(new Person()));

				await (s.FlushAsync());
				tx.Complete();
			}

			if (UseConnectionOnSystemTransactionPrepare)
			{
				Assert.That(interceptor.BeforeException, Is.Null);
			}
			else
			{
				Assert.That(interceptor.BeforeException, Is.TypeOf<HibernateException>());
			}
			// Currently always forbidden, whatever UseConnectionOnSystemTransactionEvents.
			Assert.That(interceptor.AfterException, Is.TypeOf<HibernateException>());
		}

		[Theory]
		public async Task CanUseDependentTransactionAsync(bool explicitFlush)
		{
			if (!TestDialect.SupportsDependentTransaction)
				Assert.Ignore("Dialect does not support dependent transactions");
			IgnoreIfUnsupported(explicitFlush);

			try
			{
				using (var committable = new CommittableTransaction())
				{
					sysTran.Transaction.Current = committable;
					using (var clone = committable.DependentClone(DependentCloneOption.RollbackIfNotComplete))
					{
						sysTran.Transaction.Current = clone;

						using (var s = OpenSession())
						{
							if (!AutoJoinTransaction)
								s.JoinTransaction();
							await (s.SaveAsync(new Person()));

							if (explicitFlush)
								await (s.FlushAsync());
							clone.Complete();
						}
					}

					sysTran.Transaction.Current = committable;
					committable.Commit();
				}
			}
			finally
			{
				sysTran.Transaction.Current = null;
			}
		}

		[Theory]
		public async Task CanUseSessionWithManyDependentTransactionAsync(bool explicitFlush)
		{
			if (!TestDialect.SupportsDependentTransaction)
				Assert.Ignore("Dialect does not support dependent transactions");
			IgnoreIfUnsupported(explicitFlush);
			// ODBC with SQL-Server always causes system transactions to go distributed, which causes their transaction completion to run
			// asynchronously. But ODBC enlistment also check the previous transaction in a way that do not guard against it
			// being concurrently disposed of. See https://github.com/nhibernate/nhibernate-core/pull/1505 for more details.
			if (Sfi.ConnectionProvider.Driver is OdbcDriver)
				Assert.Ignore("ODBC sometimes fails on second scope by checking the previous transaction status, which may yield an object disposed exception");
			// SAP HANA & SQL Anywhere .Net providers always cause system transactions to be distributed, causing them to
			// complete on concurrent threads. This creates race conditions when chaining scopes, the subsequent scope usage
			// finding the connection still enlisted in the previous transaction, its complete being still not finished
			// on its own thread.
			if (Sfi.ConnectionProvider.Driver is HanaDriverBase || Sfi.ConnectionProvider.Driver is SapSQLAnywhere17Driver)
				Assert.Ignore("SAP HANA and SQL Anywhere scope handling causes concurrency issues preventing chaining scope usages.");

			try
			{
				using (var s = WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
				{
					using (var committable = new CommittableTransaction())
					{
						sysTran.Transaction.Current = committable;
						using (var clone = committable.DependentClone(DependentCloneOption.RollbackIfNotComplete))
						{
							sysTran.Transaction.Current = clone;
							if (!AutoJoinTransaction)
								s.JoinTransaction();
							// Acquire the connection
							var count = await (s.Query<Person>().CountAsync());
							Assert.That(count, Is.EqualTo(0), "Unexpected initial entity count.");
							clone.Complete();
						}

						using (var clone = committable.DependentClone(DependentCloneOption.RollbackIfNotComplete))
						{
							sysTran.Transaction.Current = clone;
							if (!AutoJoinTransaction)
								s.JoinTransaction();
							await (s.SaveAsync(new Person()));

							if (explicitFlush)
								await (s.FlushAsync());

							clone.Complete();
						}

						using (var clone = committable.DependentClone(DependentCloneOption.RollbackIfNotComplete))
						{
							sysTran.Transaction.Current = clone;
							if (!AutoJoinTransaction)
								s.JoinTransaction();
							var count = await (s.Query<Person>().CountAsync());
							Assert.That(count, Is.EqualTo(1), "Unexpected entity count after committed insert.");
							clone.Complete();
						}

						sysTran.Transaction.Current = committable;
						committable.Commit();
					}
				}
			}
			finally
			{
				sysTran.Transaction.Current = null;
			}

			using (var s = OpenSession())
			{
				using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var count = await (s.Query<Person>().CountAsync());
					Assert.That(count, Is.EqualTo(1), "Unexpected entity count after global commit.");
					tx.Complete();
				}
			}
		}
	}

	[TestFixture]
	public class SystemTransactionWithoutConnectionFromPrepareFixtureAsync : SystemTransactionFixtureAsync
	{
		protected override bool UseConnectionOnSystemTransactionPrepare => false;
	}

	[TestFixture]
	public class SystemTransactionWithoutConnectionAutoEnlistmentFixtureAsync : SystemTransactionFixtureAsync
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			DisableConnectionAutoEnlist(configuration);
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> base.AppliesTo(factory) && factory.ConnectionProvider.Driver.SupportsEnlistmentWhenAutoEnlistmentIsDisabled;
	}

	[TestFixture]
	public class SystemTransactionWithoutAutoJoinTransactionAsync : SystemTransactionWithoutConnectionAutoEnlistmentFixtureAsync
	{
		protected override bool AutoJoinTransaction => false;
	}
}
