using System;
using System.Linq;
using System.Threading;
using System.Transactions;
using log4net;
using log4net.Repository.Hierarchy;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Test.TransactionTest;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	[TestFixture]
	public class DistributedSystemTransactionFixture : SystemTransactionFixtureBase
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DistributedSystemTransactionFixture));
		protected override bool UseConnectionOnSystemTransactionPrepare => true;
		protected override bool AutoJoinTransaction => true;

		protected override bool AppliesTo(Dialect.Dialect dialect)
			=> dialect.SupportsDistributedTransactions && base.AppliesTo(dialect);

		protected override void OnTearDown()
		{
			DodgeTransactionCompletionDelayIfRequired();
			base.OnTearDown();
		}

		[Test]
		public void SupportsEnlistingInDistributed()
		{
			using (new TransactionScope())
			{
				ForceEscalationToDistributedTx.Escalate();

				Assert.AreNotEqual(
					Guid.Empty,
					System.Transactions.Transaction.Current.TransactionInformation.DistributedIdentifier,
					"Transaction lacks a distributed identifier");

				using (var s = OpenSession())
				{
					s.Save(new Person());
					// Ensure the connection is acquired (thus enlisted)
					Assert.DoesNotThrow(s.Flush, "Failure enlisting a connection in a distributed transaction.");
				}
			}
		}

		[Test]
		public void SupportsPromotingToDistributed()
		{
			using (new TransactionScope())
			{
				using (var s = OpenSession())
				{
					s.Save(new Person());
					// Ensure the connection is acquired (thus enlisted)
					s.Flush();
				}
				Assert.DoesNotThrow(() => ForceEscalationToDistributedTx.Escalate(),
					"Failure promoting the transaction to distributed while already having enlisted a connection.");
				Assert.AreNotEqual(
					Guid.Empty,
					System.Transactions.Transaction.Current.TransactionInformation.DistributedIdentifier,
					"Transaction lacks a distributed identifier");
			}
		}

		[Test]
		public void WillNotCrashOnPrepareFailure()
		{
			IgnoreIfUnsupported(false);
			var tx = new TransactionScope();
			var disposeCalled = false;
			try
			{
				using (var s = OpenSession())
				{
					s.Save(new Person { NotNullData = null }); // Cause a SQL not null constraint violation.
				}

				ForceEscalationToDistributedTx.Escalate();

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
		public void CanRollbackTransaction(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			var tx = new TransactionScope();
			var disposeCalled = false;
			try
			{
				using (var s = OpenSession())
				{
					ForceEscalationToDistributedTx.Escalate(true); //will rollback tx
					s.Save(new Person());

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}
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

			AssertNoPersons();
		}

		[Theory]
		public void CanRollbackTransactionFromScope(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			using (new TransactionScope())
			using (var s = OpenSession())
			{
				ForceEscalationToDistributedTx.Escalate();
				s.Save(new Person());

				if (explicitFlush)
					s.Flush();
				// No Complete call for triggering rollback.
			}

			AssertNoPersons();
		}

		[Theory]
		[Description("Another action inside the transaction do the rollBack outside nh-session-scope.")]
		public void RollbackOutsideNh(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			try
			{
				using (var txscope = new TransactionScope())
				{
					using (var s = OpenSession())
					{
						var person = new Person();
						s.Save(person);

						if (explicitFlush)
							s.Flush();
					}
					ForceEscalationToDistributedTx.Escalate(true); //will rollback tx

					txscope.Complete();
				}

				Assert.Fail("Scope disposal has not rollback and throw.");
			}
			catch (TransactionAbortedException)
			{
				_log.Debug("Transaction aborted.");
			}

			AssertNoPersons();
		}

		[Theory]
		[Description("rollback inside nh-session-scope should not commit save and the transaction should be aborted.")]
		public void TransactionInsertWithRollBackFromScope(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			using (new TransactionScope())
			{
				using (var s = OpenSession())
				{
					var person = new Person();
					s.Save(person);
					ForceEscalationToDistributedTx.Escalate();

					if (explicitFlush)
						s.Flush();
				}
				// No Complete call for triggering rollback.
			}
			AssertNoPersons();
		}

		[Theory]
		[Description("rollback inside nh-session-scope should not commit save and the transaction should be aborted.")]
		public void TransactionInsertWithRollBackTask(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			try
			{
				using (var txscope = new TransactionScope())
				{
					using (var s = OpenSession())
					{
						var person = new Person();
						s.Save(person);
						ForceEscalationToDistributedTx.Escalate(true); //will rollback tx

						if (explicitFlush)
							s.Flush();
					}
					txscope.Complete();
				}

				Assert.Fail("Scope disposal has not rollback and throw.");
			}
			catch (TransactionAbortedException)
			{
				_log.Debug("Transaction aborted.");
			}

			AssertNoPersons();
		}

		[Theory]
		[Description(@"Two session in two txscope
 (without an explicit NH transaction)
 and with a rollback in the second dtc and a rollback outside nh-session-scope.")]
		public void TransactionInsertLoadWithRollBackFromScope(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
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
				ForceEscalationToDistributedTx.Escalate();

				// No Complete call for triggering rollback.
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(createdAt, s.Get<Person>(savedId).CreatedAt, "Entity update was not rollback-ed.");
			}
		}

		[Theory]
		[Description(@"Two session in two txscope
 (without an explicit NH transaction)
 and with a rollback in the second dtc and a ForceRollback outside nh-session-scope.")]
		public void TransactionInsertLoadWithRollBackTask(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
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

			try
			{
				using (var txscope = new TransactionScope())
				{
					using (var s = OpenSession())
					{
						var person = s.Get<Person>(savedId);
						person.CreatedAt = createdAt.AddMonths(-1);

						if (explicitFlush)
							s.Flush();
					}
					ForceEscalationToDistributedTx.Escalate(true);

					_log.Debug("completing the tx scope");
					txscope.Complete();
				}
				_log.Debug("Transaction fail.");
				Assert.Fail("Expected tx abort");
			}
			catch (TransactionAbortedException)
			{
				_log.Debug("Transaction aborted.");
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(createdAt, s.Get<Person>(savedId).CreatedAt, "Entity update was not rollback-ed.");
			}
		}

		[Test, Explicit("Test added for NH-1709 (trying to recreate the issue... without luck).")]
		public void MultiThreadedTransaction()
		{
			// Test added for NH-1709 (trying to recreate the issue... without luck)
			var mtr = new MultiThreadRunner<object>(
				20,
				o => CanRollbackTransaction(false),
				o => RollbackOutsideNh(false),
				o => TransactionInsertWithRollBackTask(false),
				o => TransactionInsertLoadWithRollBackTask(false))
			{
				EndTimeout = 5000,
				TimeoutBetweenThreadStart = 5
			};
			var totalCalls = mtr.Run(null);
			_log.DebugFormat("{0} calls", totalCalls);
			var errors = mtr.GetErrors();
			if (errors.Length > 0)
			{
				Assert.Fail("One or more thread failed, found {0} errors. First exception: {1}", errors.Length, errors[0]);
			}
		}

		[Theory]
		public void CanDeleteItemInDtc(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			object id;
			using (var tx = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					id = s.Save(new Person());

					ForceEscalationToDistributedTx.Escalate();

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}
			}

			DodgeTransactionCompletionDelayIfRequired();

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Assert.AreEqual(1, s.Query<Person>().Count(), "Entity not found in database.");
			}

			using (var tx = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					ForceEscalationToDistributedTx.Escalate();

					s.Delete(s.Get<Person>(id));

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}
			}

			DodgeTransactionCompletionDelayIfRequired();

			AssertNoPersons();
		}

		[Test]
		[Description("Open/Close a session inside a TransactionScope fails.")]
		public void NH1744()
		{
			using (new TransactionScope())
			{
				using (var s = OpenSession())
				{
					s.Flush();
				}

				using (var s = OpenSession())
				{
					s.Flush();
				}

				//and I always leave the transaction disposed without calling tx.Complete(), I let the database server to rollback all actions in this test.
			}
		}

		[Theory]
		public void CanUseSessionWithManyScopes(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			// Note that this fails with ConnectionReleaseMode.OnClose and SqlServer:
			// System.Data.SqlClient.SqlException : Microsoft Distributed Transaction Coordinator (MS DTC) has stopped this transaction.
			// Not much an issue since it is advised to not use ConnectionReleaseMode.OnClose.
			using (var s = OpenSession())
			//using (var s = Sfi.WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
			{
				using (var tx = new TransactionScope())
				{
					ForceEscalationToDistributedTx.Escalate();
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					// Acquire the connection
					var count = s.Query<Person>().Count();
					Assert.That(count, Is.EqualTo(0), "Unexpected initial entity count.");
					tx.Complete();
				}
				// No dodge here please! Allow to check chaining usages do not fail.
				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					s.Save(new Person());

					ForceEscalationToDistributedTx.Escalate();

					if (explicitFlush)
						s.Flush();

					tx.Complete();
				}

				DodgeTransactionCompletionDelayIfRequired();

				using (var tx = new TransactionScope())
				{
					ForceEscalationToDistributedTx.Escalate();
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

					ForceEscalationToDistributedTx.Escalate();

					if (explicitFlush)
						s.Flush();

					// No complete for rollback-ing.
				}

				DodgeTransactionCompletionDelayIfRequired();

				// Do not reuse the session after a rollback, its state does not allow it.
				// http://nhibernate.info/doc/nhibernate-reference/manipulatingdata.html#manipulatingdata-endingsession-commit
			}

			using (var s = OpenSession())
			{
				using (var tx = new TransactionScope())
				{
					ForceEscalationToDistributedTx.Escalate();
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					var count = s.Query<Person>().Count();
					Assert.That(count, Is.EqualTo(1), "Unexpected entity count after rollback-ed insert.");
					tx.Complete();
				}
			}
		}

		[Theory]
		public void CanUseSessionOutsideOfScopeAfterScope(bool explicitFlush)
		{
			IgnoreIfUnsupported(explicitFlush);
			// Note that this fails with ConnectionReleaseMode.OnClose and Npgsql (< 3.2.5?):
			// NpgsqlOperationInProgressException: The connection is already in state 'Executing'
			// Not much an issue since it is advised to not use ConnectionReleaseMode.OnClose.
			using (var s = OpenSession())
			//using (var s = WithOptions().ConnectionReleaseMode(ConnectionReleaseMode.OnClose).OpenSession())
			{
				using (var tx = new TransactionScope())
				{
					if (!AutoJoinTransaction)
						s.JoinTransaction();
					s.Save(new Person());

					ForceEscalationToDistributedTx.Escalate();

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

		[Theory]
		[Description("Do not fail, but warn in case a delayed after scope disposal commit is made.")]
		public void DelayedTransactionCompletion(bool explicitFlush)
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

					using (var tx = new TransactionScope())
					{
						using (var s = OpenSession())
						{
							s.Save(new Person());

							ForceEscalationToDistributedTx.Escalate();

							if (explicitFlush)
								s.Flush();
						}
						tx.Complete();
					}

					var count = controlSession.Query<Person>().Count();
					if (count != i)
					{
						// See https://github.com/npgsql/npgsql/issues/1571#issuecomment-308651461 discussion with a Microsoft
						// employee: MSDTC consider a transaction to be committed once it has collected all participant votes
						// for committing from prepare phase. It then immediately notifies all participants of the outcome.
						// This causes TransactionScope.Dispose to leave while the second phase of participants may still
						// be executing. This means the transaction from the db view point can still be pending and not yet
						// committed. This is by design of MSDTC and we have to cope with that. Some data provider may have
						// a global locking mechanism causing any subsequent use to wait for the end of the commit phase,
						// but this is not the usual case. Some other, as Npgsql < v3.2.5, may crash due to this, because
						// they re-use the connection for the second phase.
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

		// Taken and adjusted from NH1632 When_commiting_items_in_DTC_transaction_will_add_items_to_2nd_level_cache
		[Test]
		public void WhenCommittingItemsAfterSessionDisposalWillAddThemTo2ndLevelCache()
		{
			int id;
			const string notNullData = "test";
			using (var tx = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					var person = new CacheablePerson { NotNullData = notNullData };
					s.Save(person);
					id = person.Id;

					ForceEscalationToDistributedTx.Escalate();

					s.Flush();
				}
				tx.Complete();
			}

			DodgeTransactionCompletionDelayIfRequired();

			using (var tx = new TransactionScope())
			{
				using (var s = OpenSession())
				{
					ForceEscalationToDistributedTx.Escalate();

					var person = s.Load<CacheablePerson>(id);
					Assert.That(person.NotNullData, Is.EqualTo(notNullData));
				}
				tx.Complete();
			}

			// Closing the connection to ensure we can't actually use it.
			var connection = Sfi.ConnectionProvider.GetConnection();
			Sfi.ConnectionProvider.CloseConnection(connection);

			// The session is supposed to succeed because the second level cache should have the
			// entity to load, allowing the session to not use the connection at all.
			// Will fail if a transaction manager tries to enlist user supplied connection. Do
			// not add a transaction scope below.
			using (var s = WithOptions().Connection(connection).OpenSession())
			{
				CacheablePerson person = null;
				Assert.DoesNotThrow(() => person = s.Load<CacheablePerson>(id), "Failed loading entity from second level cache.");
				Assert.That(person.NotNullData, Is.EqualTo(notNullData));
			}
		}

		[Test]
		public void DoNotDeadlockOnAfterTransactionWait()
		{
			var interceptor = new AfterTransactionWaitingInterceptor();
			using (var s = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = new TransactionScope())
			{
				ForceEscalationToDistributedTx.Escalate();
				if (!AutoJoinTransaction)
					s.JoinTransaction();
				s.Save(new Person());

				s.Flush();
				tx.Complete();
			}
			Assert.That(interceptor.Exception, Is.Null);
		}

		[Test]
		public void EnforceConnectionUsageRulesOnTransactionCompletion()
		{
			var interceptor = new TransactionCompleteUsingConnectionInterceptor();
			// Do not invert session and scope, it would cause an expected failure when
			// UseConnectionOnSystemTransactionEvents is false, due to the session being closed.
			using (var s = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = new TransactionScope())
			{
				ForceEscalationToDistributedTx.Escalate();
				if (!AutoJoinTransaction)
					s.JoinTransaction();
				s.Save(new Person());

				s.Flush();
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

		[Test]
		public void AdditionalJoinDoesNotThrow()
		{
			using (new TransactionScope())
			using (var s = OpenSession())
			{
				ForceEscalationToDistributedTx.Escalate();
				Assert.DoesNotThrow(() => s.JoinTransaction());
			}
		}

		private void DodgeTransactionCompletionDelayIfRequired()
		{
			if (Sfi.ConnectionProvider.Driver.HasDelayedDistributedTransactionCompletion)
				Thread.Sleep(500);
		}

		public class ForceEscalationToDistributedTx : IEnlistmentNotification
		{
			private readonly bool _shouldRollBack;
			private readonly int _thread;

			public static void Escalate(bool shouldRollBack = false)
			{
				var force = new ForceEscalationToDistributedTx(shouldRollBack);
				System.Transactions.Transaction.Current.EnlistDurable(Guid.NewGuid(), force, EnlistmentOptions.None);
			}

			private ForceEscalationToDistributedTx(bool shouldRollBack)
			{
				_shouldRollBack = shouldRollBack;
				_thread = Thread.CurrentThread.ManagedThreadId;
			}

			public void Prepare(PreparingEnlistment preparingEnlistment)
			{
				if (_thread == Thread.CurrentThread.ManagedThreadId)
				{
					_log.Warn("Thread.CurrentThread.ManagedThreadId ({0}) is same as creation thread");
				}

				if (_shouldRollBack)
				{
					_log.Debug(">>>>Force Rollback<<<<<");
					preparingEnlistment.ForceRollback();
				}
				else
				{
					preparingEnlistment.Prepared();
				}
			}

			public void Commit(Enlistment enlistment)
			{
				enlistment.Done();
			}

			public void Rollback(Enlistment enlistment)
			{
				enlistment.Done();
			}

			public void InDoubt(Enlistment enlistment)
			{
				enlistment.Done();
			}
		}
	}

	[TestFixture]
	public class DistributedSystemTransactionWithoutConnectionFromPrepareFixture : DistributedSystemTransactionFixture
	{
		protected override bool UseConnectionOnSystemTransactionPrepare => false;
	}

	[TestFixture]
	public class DistributedSystemTransactionWithoutAutoJoinTransaction : DistributedSystemTransactionFixture
	{
		protected override bool AutoJoinTransaction => false;

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			DisableConnectionAutoEnlist(configuration);
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
			=> base.AppliesTo(factory) && factory.ConnectionProvider.Driver.SupportsEnlistmentWhenAutoEnlistmentIsDisabled;

		[Test]
		public void SessionIsNotEnlisted()
		{
			using (new TransactionScope())
			{
				ForceEscalationToDistributedTx.Escalate();
				// Dodge the OpenSession override which call JoinTransaction by calling WithOptions().
				using (var s = WithOptions().OpenSession())
				{
					Assert.That(s.GetSessionImplementation().TransactionContext, Is.Null);
				}
			}
		}
	}
}
