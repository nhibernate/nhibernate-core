using System;
using System.Collections;
using System.Threading;
using System.Transactions;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Impl;

namespace NHibernate.Transaction
{
	public class AdoNetWithDistributedTransactionFactory : ITransactionFactory
	{
		private static readonly IInternalLogger _logger = LoggerProvider.LoggerFor(typeof(ITransactionFactory));

		private readonly AdoNetTransactionFactory _adoNetTransactionFactory = new AdoNetTransactionFactory();

		public void Configure(IDictionary props)
		{
		}

		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void EnlistInDistributedTransactionIfNeeded(ISessionImplementor session)
		{
			// Ensure the session does not run on a thread supposed to be blocked, waiting
			// for transaction completion.
			session.TransactionContext?.WaitOne();
			if (session.TransactionContext != null)
			{
				return;
			}

			var transaction = System.Transactions.Transaction.Current;
			if (transaction == null)
				return;

			if (session.ConnectionManager.RequireExplicitEnlistment)
			{
				// Will fail if the connection is already enlisted in another not yet completed transaction.
				// Probable case: nested transaction scope. Supporting this could be done by releasing the
				// connection instead of enlisting.
				session.Connection.EnlistTransaction(transaction);
			}
			var transactionContext = new DistributedTransactionContext(session, transaction);
			session.TransactionContext = transactionContext;
			_logger.DebugFormat(
				"enlisted into DTC transaction: {0}",
				transactionContext.AmbientTransation.IsolationLevel);
			session.AfterTransactionBegin(null);

			transactionContext.AmbientTransation.TransactionCompleted += transactionContext.TransactionCompleted;
			transactionContext.AmbientTransation.EnlistVolatile(
				transactionContext,
				EnlistmentOptions.EnlistDuringPrepareRequired);
		}

		public bool IsInDistributedActiveTransaction(ISessionImplementor session)
		{
			var distributedTransactionContext = ((DistributedTransactionContext)session.TransactionContext);
			return distributedTransactionContext != null &&
				   distributedTransactionContext.IsInActiveTransaction;
		}

		public void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			using (var tx = new TransactionScope(TransactionScopeOption.Suppress))
			{
				// instead of duplicating the logic, we suppress the DTC transaction and create
				// our own transaction instead
				_adoNetTransactionFactory.ExecuteWorkInIsolation(session, work, transacted);
				tx.Complete();
			}
		}

		public class DistributedTransactionContext : ITransactionContext, IEnlistmentNotification
		{
			public System.Transactions.Transaction AmbientTransation { get; set; }
			public bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }

			private readonly ISessionImplementor _sessionImplementor;
			private readonly ManualResetEvent _waitEvent = new ManualResetEvent(true);
			private readonly AsyncLocal<bool> _bypassWait = new AsyncLocal<bool>();

			public bool IsInActiveTransaction;

			public DistributedTransactionContext(
				ISessionImplementor sessionImplementor,
				System.Transactions.Transaction transaction)
			{
				_sessionImplementor = sessionImplementor;
				AmbientTransation = transaction.Clone();
				IsInActiveTransaction = true;
			}

			public void WaitOne()
			{
				if (_bypassWait.Value || _isDisposed)
					return;
				try
				{
					if (!_waitEvent.WaitOne(5000))
					{
						// A call occurring after transaction scope disposal should not have to wait long, since
						// the scope disposal is supposed to block until the transaction has completed: I hope
						// that it at least ensure IO are done, even if experience shows DTC lets the scope
						// disposal leave before having finished with volatile ressources and
						// TransactionCompleted event.
						_waitEvent.Set();
						throw new HibernateException(
							"Synchronization timeout for transaction completion. This is very likely a bug in NHibernate.");
					}
				}
				catch(Exception ex)
				{
					_logger.Warn(
						"Synchronization failure, assuming it has been concurrently disposed and do not need sync anymore.",
						ex);
				}
			}

			#region IEnlistmentNotification Members

			void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
			{
				using (new SessionIdLoggingContext(_sessionImplementor.SessionId))
				{
					try
					{
						using (var tx = new TransactionScope(AmbientTransation))
						{
							_sessionImplementor.BeforeTransactionCompletion(null);
							if (_sessionImplementor.FlushMode != FlushMode.Never && _sessionImplementor.ConnectionManager.IsConnected)
							{
								using (_sessionImplementor.ConnectionManager.FlushingFromDtcTransaction)
								{
									_logger.DebugFormat("[session-id={0}] Flushing from Dtc Transaction", _sessionImplementor.SessionId);
									_sessionImplementor.Flush();
								}
							}
							_logger.Debug("prepared for DTC transaction");

							tx.Complete();
						}
						// Lock the session to ensure second phase gets done before the session is used by code following
						// the transaction scope disposal.
						_waitEvent.Reset();

						preparingEnlistment.Prepared();
					}
					catch (Exception exception)
					{
						_logger.Error("DTC transaction prepare phase failed", exception);
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
								? "committing DTC transaction"
								: "rolled back DTC transaction"
							: "DTC transaction is in doubt");
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

			public void TransactionCompleted(object sender, TransactionEventArgs e)
			{
				e.Transaction.TransactionCompleted -= TransactionCompleted;
				// This event may execute before second phase, so we cannot try to get the success from second phase.
				// Using this event is required in case the prepare phase failed and called force rollback: no second
				// phase would occur for this ressource.
				var wasSuccessful = false;
				try
				{
					wasSuccessful = e.Transaction.TransactionInformation.Status
									== TransactionStatus.Committed;
				}
				catch (ObjectDisposedException ode)
				{
					_logger.Warn("Completed transaction was disposed, assuming transaction rollback", ode);
				}
				RunAfterTransactionActions(wasSuccessful);
			}

			private volatile bool _afterTransactionActionDone;

			private void RunAfterTransactionActions(bool wasSuccessful)
			{
				if (_afterTransactionActionDone)
					// Probably called from In-Doubt and TransactionCompleted.
					return;
				// Allow transaction completed actions to run while others stay blocked.
				_bypassWait.Value = true;
				try
				{
					using (new SessionIdLoggingContext(_sessionImplementor.SessionId))
					{
						// Flag active as false before running actions, otherwise the connection manager will refuse
						// releasing the connection.
						IsInActiveTransaction = false;
						_sessionImplementor.AfterTransactionCompletion(wasSuccessful, null);
						if (ShouldCloseSessionOnDistributedTransactionCompleted)
						{
							_sessionImplementor.CloseSessionFromDistributedTransaction();
						}
						_sessionImplementor.TransactionContext = null;
					}
				}
				finally
				{
					_afterTransactionActionDone = true;
					// Dispose releases blocked threads by the way.
					// Must dispose in case !ShouldCloseSessionOnDistributedTransactionCompleted, since
					// we nullify session TransactionContext, causing it to have nothing still holding it.
					Dispose();
				}
			}

			private volatile bool _isDisposed;

			public void Dispose()
			{
				if (_isDisposed)
					// Avoid disposing twice (happen when ShouldCloseSessionOnDistributedTransactionCompleted).
					return;
				_isDisposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (AmbientTransation != null)
					{
						AmbientTransation.Dispose();
						AmbientTransation = null;
					}
					_waitEvent.Set();
					_waitEvent.Dispose();
				}
			}
		}
	}
}