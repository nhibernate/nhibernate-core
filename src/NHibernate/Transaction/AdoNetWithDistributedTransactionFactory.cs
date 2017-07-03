using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Impl;
using NHibernate.Util;

namespace NHibernate.Transaction
{
	public partial class AdoNetWithSystemTransactionFactory : ITransactionFactory
	{
		private static readonly IInternalLogger _logger = LoggerProvider.LoggerFor(typeof(ITransactionFactory));

		private readonly AdoNetTransactionFactory _adoNetTransactionFactory = new AdoNetTransactionFactory();
		private int _systemTransactionCompletionLockTimeout;

		public void Configure(IDictionary<string, string> props)
		{
			_adoNetTransactionFactory.Configure(props);
			_systemTransactionCompletionLockTimeout =
				PropertiesHelper.GetInt32(Cfg.Environment.SystemTransactionCompletionLockTimeout, props, 5000);
			if (_systemTransactionCompletionLockTimeout < -1)
				throw new HibernateException(
					$"Invalid {Cfg.Environment.SystemTransactionCompletionLockTimeout} value: {_systemTransactionCompletionLockTimeout}. It can not be less than -1.");
		}

		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void EnlistInSystemTransactionIfNeeded(ISessionImplementor session)
		{
			// Handle the transaction on the originating session only.
			var originatingSession = session.ConnectionManager.Session;

			var transaction = System.Transactions.Transaction.Current;
			if (originatingSession.TransactionContext == null ||
				// Support connection switch when connection auto-enlistment is not enabled
				originatingSession.ConnectionManager.ProcessingFromSystemTransaction)
			{
				originatingSession.ConnectionManager.EnlistIfRequired(transaction);
			}

			if (transaction == null)
				return;

			if (originatingSession.TransactionContext != null)
			{
				if (session.TransactionContext == null)
				{
					// New dependent session
					session.TransactionContext = new DependentContext(originatingSession.TransactionContext);
					session.AfterTransactionBegin(null);
				}
				return;
			}

			var transactionContext = new SystemTransactionContext(originatingSession, transaction, _systemTransactionCompletionLockTimeout);
			transactionContext.EnlistedTransaction.EnlistVolatile(
				transactionContext,
				EnlistmentOptions.EnlistDuringPrepareRequired);
			originatingSession.TransactionContext = transactionContext;

			_logger.DebugFormat(
				"enlisted into DTC transaction: {0}",
				transactionContext.EnlistedTransaction.IsolationLevel);

			originatingSession.AfterTransactionBegin(null);
			foreach (var dependentSession in originatingSession.ConnectionManager.DependentSessions)
			{
				dependentSession.TransactionContext = new DependentContext(transactionContext);
				dependentSession.AfterTransactionBegin(null);
			}
		}

		public bool IsInActiveSystemTransaction(ISessionImplementor session)
			=> session.TransactionContext?.IsInActiveTransaction ?? false;

		public void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			using (var tx = new TransactionScope(TransactionScopeOption.Suppress))
			{
				// instead of duplicating the logic, we suppress the system transaction and create
				// our own transaction instead
				_adoNetTransactionFactory.ExecuteWorkInIsolation(session, work, transacted);
				tx.Complete();
			}
		}

		public class SystemTransactionContext : ITransactionContext, IEnlistmentNotification
		{
			internal System.Transactions.Transaction EnlistedTransaction { get; }
			public bool ShouldCloseSessionOnSystemTransactionCompleted { get; set; }
			public bool IsInActiveTransaction { get; internal set; } = true;

			private readonly ISessionImplementor _sessionImplementor;
			private readonly ManualResetEventSlim _lock = new ManualResetEventSlim(true);
			private volatile bool _needCompletionLocking = true;
			// Required for not locking the completion phase itself when locking session usages from concurrent threads.
			private readonly AsyncLocal<bool> _bypassLock = new AsyncLocal<bool>();
			private readonly int _systemTransactionCompletionLockTimeout;
			private bool IsDistributed => EnlistedTransaction.TransactionInformation.DistributedIdentifier != Guid.Empty;

			public SystemTransactionContext(
				ISessionImplementor sessionImplementor,
				System.Transactions.Transaction transaction,
				int systemTransactionCompletionLockTimeout)
			{
				_sessionImplementor = sessionImplementor;
				EnlistedTransaction = transaction.Clone();
				EnlistedTransaction.TransactionCompleted += TransactionCompleted;
				_systemTransactionCompletionLockTimeout = systemTransactionCompletionLockTimeout;
			}

			public void Wait()
			{
				if (_isDisposed)
					return;
				if (_needCompletionLocking && GetTransactionStatus() != TransactionStatus.Active)
				{
					// Rollback case may end the transaction without a prepare phase, apply the lock.
					Lock();
				}

				if (_bypassLock.Value)
					return;
				try
				{
					if (_lock.Wait(_systemTransactionCompletionLockTimeout))
						return;
					// A call occurring after transaction scope disposal should not have to wait long, since
					// the scope disposal is supposed to block until the transaction has completed: I hope
					// that it at least ensures IO are done, even if experience shows DTC lets the scope
					// disposal leave before having finished with volatile ressources and
					// TransactionCompleted event.
					// Remove the block then throw.
					Unlock();
					throw new HibernateException(
						$"Synchronization timeout for transaction completion. Either raise {Cfg.Environment.SystemTransactionCompletionLockTimeout}, or this may be a bug in NHibernate.");
				}
				catch (HibernateException)
				{
					throw;
				}
				catch (Exception ex)
				{
					_logger.Warn(
						"Synchronization failure, assuming it has been concurrently disposed and does not need sync anymore.",
						ex);
				}
			}

			private void Lock()
			{
				if (!_needCompletionLocking || _isDisposed)
					return;
				_needCompletionLocking = false;
				_lock.Reset();
			}

			private void Unlock()
			{
				_lock.Set();
			}

			/// <summary>
			/// Safely get the <see cref="TransactionStatus"/> of the context transaction.
			/// </summary>
			/// <returns>The <see cref="TransactionStatus"/> of the context transaction, or <see langword="null"/>
			/// if it cannot be obtained.</returns>
			/// <remarks>The status may no more be obtainable during transaction completion events in case of
			/// rollback.</remarks>
			protected TransactionStatus? GetTransactionStatus()
			{
				try
				{
					return EnlistedTransaction.TransactionInformation.Status;
				}
				catch (ObjectDisposedException ode)
				{
					_logger.Warn("Completed transaction was already disposed, unable to get its status.", ode);
					return null;
				}
			}

			#region IEnlistmentNotification Members

			void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
			{
				using (new SessionIdLoggingContext(_sessionImplementor.SessionId))
				{
					try
					{
						using (var tx = new TransactionScope(EnlistedTransaction))
						{
							if (_sessionImplementor.ConnectionManager.IsConnected)
							{
								using (_sessionImplementor.ConnectionManager.BeginFlushingFromSystemTransaction(IsDistributed))
								{
									_sessionImplementor.BeforeTransactionCompletion(null);
									foreach (var dependentSession in _sessionImplementor.ConnectionManager.DependentSessions)
										dependentSession.BeforeTransactionCompletion(null);

									_logger.Debug("prepared for system transaction");

									tx.Complete();
								}
							}
						}
						// Lock the session to ensure second phase gets done before the session is used by code following
						// the transaction scope disposal.
						Lock();

						preparingEnlistment.Prepared();
					}
					catch (Exception exception)
					{
						_logger.Error("System transaction prepare phase failed", exception);
						preparingEnlistment.ForceRollback(exception);
					}
				}
			}

			void IEnlistmentNotification.Commit(Enlistment enlistment)
				=> ProcessSecondPhase(enlistment, true);

			void IEnlistmentNotification.Rollback(Enlistment enlistment)
				=> ProcessSecondPhase(enlistment, false);

			void IEnlistmentNotification.InDoubt(Enlistment enlistment)
				=> ProcessSecondPhase(enlistment, null);

			private void ProcessSecondPhase(Enlistment enlistment, bool? success)
			{
				using (new SessionIdLoggingContext(_sessionImplementor.SessionId))
				{
					_logger.Debug(
						success.HasValue
							? success.Value
								? "Committing system transaction"
								: "Rolled back system transaction"
							: "System transaction is in doubt");
					// we have not much to do here, since it is the actual
					// DB connection that will commit/rollback the transaction
					// Usual cases will raise after transaction actions from TransactionCompleted event.
					if (!success.HasValue)
					{
						// In-doubt. A durable ressource has failed and may recover, but we won't wait to know.
						RunAfterTransactionActions(false);
					}

					enlistment.Done();
				}
			}

			#endregion

			private void TransactionCompleted(object sender, TransactionEventArgs e)
			{
				EnlistedTransaction.TransactionCompleted -= TransactionCompleted;
				// This event may execute before second phase, so we cannot try to get the success from second phase.
				// Using this event is required by example in case the prepare phase failed and called force rollback:
				// no second phase would occur for this ressource. Maybe this may happen in some other circumstances
				// too.
				var wasSuccessful = GetTransactionStatus() == TransactionStatus.Committed;

				RunAfterTransactionActions(wasSuccessful);
			}

			private volatile bool _afterTransactionActionsDone;

			private void RunAfterTransactionActions(bool wasSuccessful)
			{
				if (_afterTransactionActionsDone)
					// Probably called from In-Doubt and TransactionCompleted.
					return;
				_afterTransactionActionsDone = true;
				// Allow transaction completed actions to run while others stay blocked.
				_bypassLock.Value = true;
				try
				{
					using (new SessionIdLoggingContext(_sessionImplementor.SessionId))
					{
						// Flag active as false before running actions, otherwise the connection manager will refuse
						// releasing the connection.
						IsInActiveTransaction = false;
						_sessionImplementor.ConnectionManager.AfterTransaction();
						_sessionImplementor.AfterTransactionCompletion(wasSuccessful, null);
						foreach (var dependentSession in _sessionImplementor.ConnectionManager.DependentSessions)
							dependentSession.AfterTransactionCompletion(wasSuccessful, null);

						Cleanup(_sessionImplementor);
					}
				}
				finally
				{
					// Dispose releases blocked threads by the way.
					Dispose();
				}
			}

			private static void Cleanup(ISessionImplementor session)
			{
				foreach (var dependentSession in session.ConnectionManager.DependentSessions.ToList())
				{
					var dependentContext = dependentSession.TransactionContext;
					// Avoid a race condition with session disposal. (Protected on session side by WaitOne,
					// but better have more safety.)
					dependentSession.TransactionContext = null;
					if (dependentContext == null)
						continue;
					if (dependentContext.ShouldCloseSessionOnSystemTransactionCompleted)
						// This changes the enumerated collection.
						dependentSession.CloseSessionFromSystemTransaction();
					dependentContext.Dispose();
				}
				var context = session.TransactionContext;
				// Avoid a race condition with session disposal. (Protected on session side by WaitOne,
				// but better have more safety.)
				session.TransactionContext = null;
				if (context.ShouldCloseSessionOnSystemTransactionCompleted)
				{
					// This closes the connection manager, which will release the connection.
					// This can cause issues with the connection own second phase and concurrency issues
					// when the transaction is distributed. In such case, user needs to disable
					// UseConnectionOnSystemTransactionPrepare.
					session.CloseSessionFromSystemTransaction();
				}
				// No dispose, done later.
			}

			private volatile bool _isDisposed;

			public void Dispose()
			{
				if (_isDisposed)
					// Avoid disposing twice.
					return;
				_isDisposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					Unlock();
					EnlistedTransaction?.Dispose();
					_lock.Dispose();
				}
			}
		}

		public class DependentContext : ITransactionContext
		{
			public bool IsInActiveTransaction
				=> _mainTransactionContext.IsInActiveTransaction;

			public bool ShouldCloseSessionOnSystemTransactionCompleted { get; set; }

			private readonly ITransactionContext _mainTransactionContext;

			public DependentContext(ITransactionContext mainTransactionContext)
			{
				_mainTransactionContext = mainTransactionContext;
			}

			public void Wait() =>
				_mainTransactionContext.Wait();

			public void Dispose() { }
		}
	}
}
