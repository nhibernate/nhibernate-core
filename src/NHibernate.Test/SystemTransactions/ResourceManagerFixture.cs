using System;
using System.IO;
using System.Threading;
using System.Transactions;
using log4net;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

using SysTran = System.Transactions.Transaction;

namespace NHibernate.Test.SystemTransactions
{
	/// <summary>
	/// Holds tests for checking MSDTC resource managers behavior. They are not actual NHibernate tests,
	/// they are here to help understand how NHibernate should implement its own resource manager.
	/// </summary>
	[TestFixture]
	[Explicit("Does not test NHibernate but MSDTC")]
	public class ResourceManagerFixture
	{
		#region Distributed

		// All these tests demonstrates the asynchronism of MSDTC.
		// - Prepare phases of resources run concurrently to each other, in no predictable order. But they may
		// not be concurrent with code following the scope disposal.
		// - Second phases and transaction complete events are fully concurrent. Second phase may run in
		// parallel with complete event of the same resource. Second phases of different resources run in
		// parallel too, ... All that in concurrence with code following the scope disposal.

		#region Commit

		// enlistInPrepare enable this option for the "session" resource. In such case, the "session"
		// resource is prepared before others in all commit tests. So we could count on this for not
		// opening a new connection and instead use the current one, but some reports (NH-2238, NH-3968)
		// indicates this may not always be true.

		[Test]
		public void DistributedCommit([Values(false, true)] bool enlistInPrepare)
		{
			using (var scope = new TransactionScope())
			{
				_log.InfoFormat(
					"Scope opened, id {0}, distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate a simple connection: durable resource supporting single phase.
				// (Note that SQL Server 2005 and above use IPromotableSinglePhaseNotification
				// for delegating the resource management to the SQL server.)
				EnlistResource.EnlistDurable(false, true);
				_log.InfoFormat(
					"Fake connection opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate another resource, not even supporting single phase
				EnlistResource.EnlistDurable();
				_log.InfoFormat(
					"Fake other resource, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate NHibernate : volatile no single phase support
				if (enlistInPrepare)
					EnlistResource.EnlistWithPrepareEnlistmentVolatile();
				else
					EnlistResource.EnlistVolatile();
				_log.InfoFormat(
					"Fake session opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				scope.Complete();
				_log.Info("Scope completed");
			}
			_log.Info("Scope disposed");
		}

		[Test]
		public void DistributedNpgsqlCommit([Values(false, true)] bool enlistInPrepare)
		{
			using (var scope = new TransactionScope())
			{
				_log.InfoFormat(
					"Scope opened, id {0}, distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate a Npgsql connection: as of Npgsql 3.2.4, volatile resource with single phase support
				EnlistResource.EnlistVolatile(false, true);
				_log.InfoFormat(
					"Fake connection opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate another resource, not even supporting single phase (required for going distributed with "Npgsql")
				EnlistResource.EnlistDurable();
				_log.InfoFormat(
					"Fake other resource, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate NHibernate : volatile no single phase support
				if (enlistInPrepare)
					EnlistResource.EnlistWithPrepareEnlistmentVolatile();
				else
					EnlistResource.EnlistVolatile();
				_log.InfoFormat(
					"Fake session opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				scope.Complete();
				_log.Info("Scope completed");
			}
			_log.Info("Scope disposed");
		}

		#endregion

		#region Failure

		[Test]
		[Explicit("Failing.")]
		public void DistributedTransactionStatusMustBeInactiveAfterRollbackedScope()
		{
			SysTran transaction = null;
			try
			{
				using (CreateDistributedTransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// The trouble occurs only with cloned transaction. The original one is disposed before and so
					// considered inactive by FailsafeGetTransactionStatus test.
					transaction = SysTran.Current.Clone();
					_log.Info("Scope not completed");
				}
				_log.Info("Scope disposed");
				Assert.That(FailsafeGetTransactionStatus(transaction), Is.Not.EqualTo(TransactionStatus.Active));
			}
			finally
			{
				transaction?.Dispose();
			}
		}

		[Test]
		[Explicit("Failing")]
		public void DistributedTransactionFromCompletionEventShouldBeTheOneToWhichTheEventIsAttached()
		{
			SysTran clone = null;
			SysTran eventTransaction = null;
			try
			{
				using (CreateDistributedTransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					clone = SysTran.Current.Clone();
					clone.TransactionCompleted += Clone_TransactionCompleted;
					_log.Info("Scope not completed");
				}
				_log.Info("Scope disposed");
				while (eventTransaction == null)
					Thread.Sleep(10);
				_log.Info("Event transaction received");
				Assert.That(eventTransaction, Is.SameAs(clone));
			}
			finally
			{
				clone?.Dispose();
			}

			void Clone_TransactionCompleted(object sender, TransactionEventArgs e)
			{
				eventTransaction = e.Transaction;
			}
		}

		// Failing in phase 2 seems almost a no-op. But it indeed has averse effects: the transaction will be marked
		// as "Cannot Notify" and will not be cleared from MSDTC logs.
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms681727.aspx
		// A failure in phase 2 is to be avoided.
		[Test]
		[Explicit("Causes a transaction to remain in MSDTC logs. Clean-it with comexp.msc.")]
		public void DistributedFailureInSecondPhase()
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(5)))
			{
				_log.InfoFormat(
					"Scope opened, id {0}, distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate a failing connection
				EnlistResource.EnlistSecondPhaseFailingDurable();
				_log.InfoFormat(
					"Fake connection opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate another resource, not even supporting single phase
				EnlistResource.EnlistDurable();
				_log.InfoFormat(
					"Fake other resource, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate NHibernate : volatile no single phase support + enlist in prepare option
				EnlistResource.EnlistWithPrepareEnlistmentVolatile();
				_log.InfoFormat(
					"Fake session opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				scope.Complete();
				_log.Info("Scope completed");
			}
			_log.Info("Scope disposed");
		}

		// Fails because throwing exception from prepare, without notifying the enlistment.
		// MSDTC then just wait the timeout. Demonstrates enlistment must always be notified,
		// failure is not an option.
		[Test]
		public void DistributedInDoubtFailure()
		{
			try
			{
				using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(5)))
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate a simple connection: durable resource supporting single phase.
					// (Note that SQL Server 2005 and above use IPromotableSinglePhaseNotification
					// for delegating the resource management to the SQL server.)
					EnlistResource.EnlistInDoubtDurable();
					_log.InfoFormat(
						"Fake connection opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate another resource, not even supporting single phase
					EnlistResource.EnlistDurable();
					_log.InfoFormat(
						"Fake other resource, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate NHibernate : volatile no single phase support + enlist in prepare option
					EnlistResource.EnlistWithPrepareEnlistmentVolatile();
					_log.InfoFormat(
						"Fake session opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					scope.Complete();
					_log.Info("Scope completed");
				}
			}
			catch (TransactionAbortedException)
			{
				// expected
			}
			_log.Info("Scope disposed");
		}

		#endregion

		#region Rollback

		// Demonstrates that in rollback cases, the prepare phases may not be called at all. If we have to lock
		// the session from being used till second phase end, we cannot rely on prepare phase for this.

		[Test]
		public void DistributedRollback([Values(false, true)] bool fromConnection, [Values(false, true)] bool fromOther, [Values(false, true)] bool fromSession)
		{
			var shouldFail = fromConnection || fromSession || fromOther;
			try
			{
				using (var scope = new TransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate a simple connection: durable resource supporting single phase.
					// (Note that SQL Server 2005 and above use IPromotableSinglePhaseNotification
					// for delegating the resource management to the SQL server.)
					EnlistResource.EnlistDurable(fromConnection, true);
					_log.InfoFormat(
						"Fake connection opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate another resource, not even supporting single phase
					EnlistResource.EnlistDurable(fromOther);
					_log.InfoFormat(
						"Fake other resource, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate NHibernate : volatile no single phase support + enlist in prepare option
					EnlistResource.EnlistWithPrepareEnlistmentVolatile(fromSession);
					_log.InfoFormat(
						"Fake session opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					if (shouldFail)
					{
						scope.Complete();
						_log.Info("Scope completed");
					}
					else
						_log.Info("Scope not completed for triggering rollback");
				}
			}
			catch (TransactionAbortedException)
			{
				if (!shouldFail)
					throw;
			}
			_log.Info("Scope disposed");
		}

		[Test]
		public void DistributedNpgsqlRollback([Values(false, true)] bool fromConnection, [Values(false, true)] bool fromOther, [Values(false, true)] bool fromSession)
		{
			var shouldFail = fromConnection || fromSession || fromOther;
			try
			{
				using (var scope = new TransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate a Npgsql connection: as of Npgsql 3.2.4, volatile resource with single phase support
					EnlistResource.EnlistVolatile(fromConnection, true);
					_log.InfoFormat(
						"Fake connection opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate another resource, not even supporting single phase (required for going distributed with "Npgsql")
					EnlistResource.EnlistDurable(fromOther);
					_log.InfoFormat(
						"Fake other resource, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate NHibernate : volatile no single phase support + enlist in prepare option
					EnlistResource.EnlistWithPrepareEnlistmentVolatile(fromSession);
					_log.InfoFormat(
						"Fake session opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					if (shouldFail)
					{
						scope.Complete();
						_log.Info("Scope completed");
					}
					else
						_log.Info("Scope not completed for triggering rollback");
				}
			}
			catch (TransactionAbortedException)
			{
				if (!shouldFail)
					throw;
			}
			_log.Info("Scope disposed");
		}

		[Test]
		public void DistributedTransactionStatusFromCompletionEventShouldNotBeActiveOnRollback()
		{
			SysTran clone = null;
			SysTran eventTransaction = null;
			TransactionStatus? cloneStatusAtCompletion = null;
			try
			{
				using (CreateDistributedTransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					clone = SysTran.Current.Clone();
					clone.TransactionCompleted += Clone_TransactionCompleted;
					_log.Info("Scope not completed");
				}
				_log.Info("Scope disposed");
				while (eventTransaction == null)
					Thread.Sleep(10);
				_log.Info("Event transaction received");
				Assert.That(cloneStatusAtCompletion, Is.Not.EqualTo(TransactionStatus.Active));
			}
			finally
			{
				clone?.Dispose();
			}

			void Clone_TransactionCompleted(object sender, TransactionEventArgs e)
			{
				cloneStatusAtCompletion = FailsafeGetTransactionStatus(clone);
				eventTransaction = e.Transaction;
			}
		}

		#endregion

		#endregion

		#region Non distributed

		// No asynchronism to be seen in those cases.

		#region Commit

		// If this case was the sole case for non-distributed scopes, we could optimize the flush from
		// prepare by re-using the main connection, since this case showcase that durable resource
		// supporting single phase are single phase executed after volatile resource prepare phase.
		// Unfortunately this is not the sole case. Unless checking code of each provider, we should
		// use another connection for the flush from prepare even in non-distributed cases.
		[Test]
		public void NonDistributedCommit([Values(false, true)] bool enlistInPrepare)
		{
			using (var scope = new TransactionScope())
			{
				_log.InfoFormat(
					"Scope opened, id {0}, distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate a simple connection: durable resource supporting single phase.
				// (Note that SQL Server 2005 and above use IPromotableSinglePhaseNotification
				// for delegating the resource management to the SQL server.)
				EnlistResource.EnlistDurable(false, true);
				_log.InfoFormat(
					"Fake connection opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate NHibernate : volatile no single phase support
				if (enlistInPrepare)
					EnlistResource.EnlistWithPrepareEnlistmentVolatile();
				else
					EnlistResource.EnlistVolatile();
				_log.InfoFormat(
					"Fake session opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				scope.Complete();
				_log.Info("Scope completed");
			}
			_log.Info("Scope disposed");
		}

		// This one led to reporting https://github.com/npgsql/npgsql/issues/1625, when enlistInPrepare
		// is false. In this case, the single phase optimization of the "connection" is not called,
		// causing it to go through 2PC.
		[Test]
		public void NonDistributedNpgsqlCommit([Values(false, true)] bool enlistInPrepare)
		{
			using (var scope = new TransactionScope())
			{
				_log.InfoFormat(
					"Scope opened, id {0}, distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate a Npgsql connection: as of Npgsql 3.2.4, volatile resource with single phase support
				EnlistResource.EnlistVolatile(false, true);
				_log.InfoFormat(
					"Fake connection opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				// Simulate NHibernate : volatile no single phase support
				if (enlistInPrepare)
					EnlistResource.EnlistWithPrepareEnlistmentVolatile();
				else
					EnlistResource.EnlistVolatile();
				_log.InfoFormat(
					"Fake session opened, scope id {0} and distributed id {1}",
					SysTran.Current.TransactionInformation.LocalIdentifier,
					SysTran.Current.TransactionInformation.DistributedIdentifier);
				scope.Complete();
				_log.Info("Scope completed");
			}
			_log.Info("Scope disposed");
		}

		#endregion

		#region In-doubt

		// This case indicates that the complete event is triggered in in-doubt case. Firing transaction
		// completion from in-doubt and complete case seems to have no grounding.
		// Since failures in notifications can fill MSDTC logs till hanging it, better move all eligible
		// processing to transaction completed. So we should do transaction completion only in
		// complete event, no more in in-doubt.
		[Test]
		public void NonDistributedInDoubt()
		{
			try
			{
				using (var scope = new TransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate a simple connection: durable resource supporting single phase.
					// (Note that SQL Server 2005 and above use IPromotableSinglePhaseNotification
					// for delegating the resource management to the SQL server.)
					EnlistResource.EnlistInDoubtDurable();
					_log.InfoFormat(
						"Fake connection opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate NHibernate : volatile no single phase support + enlist in prepare option
					EnlistResource.EnlistWithPrepareEnlistmentVolatile();
					_log.InfoFormat(
						"Fake session opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					scope.Complete();
					_log.Info("Scope completed");
				}
			}
			catch (TransactionInDoubtException)
			{
				// expected
			}
			_log.Info("Scope disposed");
		}

		#endregion

		#region Rollback

		[Test]
		public void TransactionStatusMustBeInactiveAfterRollbackedScope()
		{
			SysTran transaction = null;
			try
			{
				using (new TransactionScope())
				{
					transaction = SysTran.Current.Clone();
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
				}
				_log.Info("Scope disposed");
				Assert.That(FailsafeGetTransactionStatus(transaction), Is.Not.EqualTo(TransactionStatus.Active));
			}
			finally
			{
				transaction?.Dispose();
			}
		}

		[Test]
		public void NonDistributedRollback([Values(false, true)] bool fromConnection, [Values(false, true)] bool fromSession)
		{
			var shouldFail = fromConnection || fromSession;
			try
			{
				using (var scope = new TransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate a simple connection: durable resource supporting single phase.
					// (Note that SQL Server 2005 and above use IPromotableSinglePhaseNotification
					// for delegating the resource management to the SQL server.)
					EnlistResource.EnlistDurable(fromConnection, true);
					_log.InfoFormat(
						"Fake connection opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate NHibernate : volatile no single phase support + enlist in prepare option
					EnlistResource.EnlistWithPrepareEnlistmentVolatile(fromSession);
					_log.InfoFormat(
						"Fake session opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					if (shouldFail)
					{
						scope.Complete();
						_log.Info("Scope completed");
					}
					else
						_log.Info("Scope not completed for triggering rollback");
				}
			}
			catch (TransactionAbortedException)
			{
				if (!shouldFail)
					throw;
			}
			_log.Info("Scope disposed");
		}

		[Test]
		public void NonDistributedNpgsqlRollback([Values(false, true)] bool fromConnection, [Values(false, true)] bool fromSession)
		{
			var shouldFail = fromConnection || fromSession;
			try
			{
				using (var scope = new TransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate a Npgsql connection: as of Npgsql 3.2.4, volatile resource with single phase support
					EnlistResource.EnlistVolatile(fromConnection, true);
					_log.InfoFormat(
						"Fake connection opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					// Simulate NHibernate : volatile no single phase support + enlist in prepare option
					EnlistResource.EnlistWithPrepareEnlistmentVolatile(fromSession);
					_log.InfoFormat(
						"Fake session opened, scope id {0} and distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					if (shouldFail)
					{
						scope.Complete();
						_log.Info("Scope completed");
					}
					else
						_log.Info("Scope not completed for triggering rollback");
				}
			}
			catch (TransactionAbortedException)
			{
				if (!shouldFail)
					throw;
			}
			_log.Info("Scope disposed");
		}

		#endregion

		#region Failure

		[Test]
		[Explicit("Failing")]
		public void TransactionFromCompletionEventShouldBeTheOneToWhichTheEventIsAttached()
		{
			SysTran clone = null;
			SysTran eventTransaction = null;
			try
			{
				using (new TransactionScope())
				{
					_log.InfoFormat(
						"Scope opened, id {0}, distributed id {1}",
						SysTran.Current.TransactionInformation.LocalIdentifier,
						SysTran.Current.TransactionInformation.DistributedIdentifier);
					clone = SysTran.Current.Clone();
					clone.TransactionCompleted += Clone_TransactionCompleted;
					_log.Info("Scope not completed");
				}
				_log.Info("Scope disposed");
				while (eventTransaction == null)
					Thread.Sleep(10);
				_log.Info("Event transaction received");
				Assert.That(eventTransaction, Is.SameAs(clone));
			}
			finally
			{
				clone?.Dispose();
			}

			void Clone_TransactionCompleted(object sender, TransactionEventArgs e)
			{
				eventTransaction = e.Transaction;
			}
		}

		#endregion

		#endregion

		#region Tests setup/teardown/utils

		private static readonly ILog _log = LogManager.GetLogger(typeof(ResourceManagerFixture));
		private LogSpy _spy;

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			_spy = new LogSpy(_log);
			_spy.Appender.Layout = new PatternLayout("%d{ABSOLUTE} [%t] - %m%n");
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			_spy.Dispose();
		}

		[SetUp]
		public void SetUp()
		{
			EnlistResource.Counter = 0;
		}

		[TearDown]
		public void TearDown()
		{
			// Account for MSDTC async second phase, for collecting all logs
			Thread.Sleep(200);

			using (var wholeMessage = new StringWriter())
			{
				foreach (var loggingEvent in _spy.Appender.PopAllEvents())
				{
					_spy.Appender.Layout.Format(wholeMessage, loggingEvent);
				}
				// R# console ignores logs from other threads.
				_log.Info(
					@"

All threads log:
" + wholeMessage);
			}
			_spy.Appender.Clear();
		}

		// Taken from NH-3023 test.
		private static TransactionScope CreateDistributedTransactionScope()
		{
			var scope = new TransactionScope();
			//
			// Forces promotion to distributed transaction
			//
			TransactionInterop.GetTransmitterPropagationToken(System.Transactions.Transaction.Current);
			return scope;
		}

		private static TransactionStatus? FailsafeGetTransactionStatus(SysTran transaction)
		{
			try
			{
				return transaction.TransactionInformation.Status;
			}
			catch (Exception ex)
			{
				// Only log exception message for avoid bloating the log for a minor case
				_log.InfoFormat("Failed getting transaction status, {0}", ex.Message);
				return null;
			}
		}

		public class EnlistResource : IEnlistmentNotification
		{
			// Causes concurrency to be more obvious.
			public static int SleepTime { get; set; } = 2;

			public static int Counter { get; set; }

			protected bool ShouldRollBack { get; }
			protected bool ShouldGoInDoubt { get; }
			protected bool FailInSecondPhase { get; }
			protected string Name { get; }

			public static void EnlistVolatile(bool shouldRollBack = false)
				=> EnlistVolatile(shouldRollBack, false);

			public static void EnlistVolatile(bool shouldRollBack, bool supportsSinglePhase)
				=> Enlist(false, supportsSinglePhase, shouldRollBack);

			public static void EnlistWithPrepareEnlistmentVolatile(bool shouldRollBack = false)
				=> Enlist(false, false, shouldRollBack, false, false, true);

			public static void EnlistDurable(bool shouldRollBack = false)
				=> EnlistDurable(shouldRollBack, false);

			public static void EnlistDurable(bool shouldRollBack, bool supportsSinglePhase)
				=> Enlist(true, supportsSinglePhase, shouldRollBack);

			public static void EnlistInDoubtDurable()
				=> Enlist(true, true, false, true);

			public static void EnlistSecondPhaseFailingDurable()
				=> Enlist(true, false, false, false, true);

			private static void Enlist(bool durable, bool supportsSinglePhase, bool shouldRollBack, bool inDoubt = false,
				bool failInSecondPhase = false, bool enlistInPrepareOption = false)
			{
				Counter++;

				var name = $"{(durable ? "Durable" : "Volatile")} resource {Counter}";
				EnlistResource resource;
				var options = enlistInPrepareOption ? EnlistmentOptions.EnlistDuringPrepareRequired : EnlistmentOptions.None;
				if (supportsSinglePhase)
				{
					var spResource = new EnlistSinglePhaseResource(shouldRollBack, name, inDoubt, failInSecondPhase);
					resource = spResource;
					if (durable)
						SysTran.Current.EnlistDurable(Guid.NewGuid(), spResource, options);
					else
						SysTran.Current.EnlistVolatile(spResource, options);
				}
				else
				{
					resource = new EnlistResource(shouldRollBack, name, inDoubt, failInSecondPhase);
					// Not duplicate code with above, that is not the same overload which ends up called.
					if (durable)
						SysTran.Current.EnlistDurable(Guid.NewGuid(), resource, options);
					else
						SysTran.Current.EnlistVolatile(resource, options);
				}

				SysTran.Current.TransactionCompleted += resource.Current_TransactionCompleted;

				_log.Info(name + ": enlisted");
			}

			protected EnlistResource(bool shouldRollBack, string name, bool inDoubt, bool failInSecondPhase)
			{
				ShouldRollBack = shouldRollBack;
				ShouldGoInDoubt = inDoubt;
				FailInSecondPhase = failInSecondPhase;
				Name = name;
			}

			public void Prepare(PreparingEnlistment preparingEnlistment)
			{
				_log.Info(Name + ": prepare phase start");
				Thread.Sleep(SleepTime);
				if (ShouldRollBack)
				{
					_log.Info(Name + ": prepare phase, calling rollback-ed");
					preparingEnlistment.ForceRollback();
				}
				else if (ShouldGoInDoubt)
				{
					throw new InvalidOperationException("In-doubt mode currently supported only by durable in single phase.");
				}
				else
				{
					_log.Info(Name + ": prepare phase, calling prepared");
					preparingEnlistment.Prepared();
				}
				Thread.Sleep(SleepTime);
				_log.Info(Name + ": prepare phase end");
			}

			public void Commit(Enlistment enlistment)
			{
				_log.Info(Name + ": commit phase start");
				Thread.Sleep(SleepTime);
				if (FailInSecondPhase)
					throw new InvalidOperationException("Asked to fail");
				_log.Info(Name + ": commit phase, calling done");
				enlistment.Done();
				Thread.Sleep(SleepTime);
				_log.Info(Name + ": commit phase end");
			}

			public void Rollback(Enlistment enlistment)
			{
				_log.Info(Name + ": rollback phase start");
				Thread.Sleep(SleepTime);
				if (FailInSecondPhase)
					throw new InvalidOperationException("Asked to fail");
				_log.Info(Name + ": rollback phase, calling done");
				enlistment.Done();
				Thread.Sleep(SleepTime);
				_log.Info(Name + ": rollback phase end");
			}

			public void InDoubt(Enlistment enlistment)
			{
				_log.Info(Name + ": in-doubt phase start");
				Thread.Sleep(SleepTime);
				if (FailInSecondPhase)
					throw new InvalidOperationException("Asked to fail");
				_log.Info(Name + ": in-doubt phase, calling done");
				enlistment.Done();
				Thread.Sleep(SleepTime);
				_log.Info(Name + ": in-doubt phase end");
			}

			private void Current_TransactionCompleted(object sender, TransactionEventArgs e)
			{
				_log.Info(Name + ": transaction completed start");
				Thread.Sleep(SleepTime);
				_log.Info(Name + ": transaction completed middle");
				Thread.Sleep(SleepTime);
				_log.Info(Name + ": transaction completed end");
			}

			private class EnlistSinglePhaseResource : EnlistResource, ISinglePhaseNotification
			{
				public EnlistSinglePhaseResource(bool shouldRollBack, string name, bool inDoubt, bool failInSecondPhase) :
					base(shouldRollBack, "Single phase " + name, inDoubt, failInSecondPhase)
				{
				}

				public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
				{
					_log.Info(Name + ": transaction single phase start");
					Thread.Sleep(SleepTime);
					if (ShouldRollBack)
					{
						_log.Info(Name + ": transaction single phase, calling aborted");
						singlePhaseEnlistment.Aborted();
					}
					else if (ShouldGoInDoubt)
					{
						_log.Info(Name + ": transaction single phase, calling in doubt");
						singlePhaseEnlistment.InDoubt();
					}
					else
					{
						_log.Info(Name + ": transaction single phase, calling committed");
						singlePhaseEnlistment.Committed();
					}
					Thread.Sleep(SleepTime);
					_log.Info(Name + ": transaction single phase end");
				}
			}
		}

		#endregion
	}
}
