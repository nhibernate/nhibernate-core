using System;
using System.Collections;
using System.Transactions;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Impl;

namespace NHibernate.Transaction
{
	public class AdoNetWithDistributedTransactionFactory : ITransactionFactory
	{
		private static readonly IInternalLogger logger = LoggerProvider.LoggerFor(typeof(ITransactionFactory));

		private readonly AdoNetTransactionFactory adoNetTransactionFactory = new AdoNetTransactionFactory();

		public void Configure(IDictionary props)
		{
		}

		public ITransaction CreateTransaction(ISessionImplementor session)
		{
			return new AdoTransaction(session);
		}

		public void EnlistInDistributedTransactionIfNeeded(ISessionImplementor session)
		{
			if (session.TransactionContext != null)
				return;

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
			logger.DebugFormat("enlisted into DTC transaction: {0}",
							   transactionContext.AmbientTransation.IsolationLevel);
			session.AfterTransactionBegin(null);

			transactionContext.AmbientTransation.EnlistVolatile(transactionContext,
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
				adoNetTransactionFactory.ExecuteWorkInIsolation(session, work, transacted);
				tx.Complete();
			}
		}

		public class DistributedTransactionContext : ITransactionContext, IEnlistmentNotification
		{
			public System.Transactions.Transaction AmbientTransation { get; set; }
			public bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }
			private readonly ISessionImplementor sessionImplementor;
			public bool IsInActiveTransaction;

			public DistributedTransactionContext(ISessionImplementor sessionImplementor, System.Transactions.Transaction transaction)
			{
				this.sessionImplementor = sessionImplementor;
				AmbientTransation = transaction.Clone();
				IsInActiveTransaction = true;
			}

			#region IEnlistmentNotification Members

			void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
			{
				using (new SessionIdLoggingContext(sessionImplementor.SessionId))
				{
					try
					{
						using (var tx = new TransactionScope(AmbientTransation))
						{
							sessionImplementor.BeforeTransactionCompletion(null);
							if (sessionImplementor.FlushMode != FlushMode.Never && sessionImplementor.ConnectionManager.IsConnected)
							{
								using (sessionImplementor.ConnectionManager.FlushingFromDtcTransaction)
								{
									logger.Debug(string.Format("[session-id={0}] Flushing from Dtc Transaction", sessionImplementor.SessionId));
									sessionImplementor.Flush();
								}
							}
							logger.Debug("prepared for DTC transaction");

							tx.Complete();
						}
						preparingEnlistment.Prepared();
					}
					catch (Exception exception)
					{
						logger.Error("DTC transaction prepare phase failed", exception);
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
				using (new SessionIdLoggingContext(sessionImplementor.SessionId))
				{
					logger.Debug(success.HasValue
						? success.Value ? "committing DTC transaction" : "rolled back DTC transaction"
						: "DTC transaction is in doubt");
					// we have not much to do here, since it is the actual
					// DB connection that will commit/rollback the transaction
					IsInActiveTransaction = false;
					// In doubt means the transaction may get carried on successfully, but maybe one hour later, the
					// time for the failing durable ressource to come back online and tell. We won't wait for knowing,
					// so better be pessimist.
					var signalSuccess = success ?? false;
					// May fail by releasing the connection while the connection has its own second phase to do.
					// Since we can release connection before completing an ambient transaction, maybe it will never
					// fail, but here we are at the transaction completion stage, which is not documented for
					// supporting this. See next comment as for why we cannot do that within
					// TransactionCompletion event.
					sessionImplementor.AfterTransactionCompletion(signalSuccess, null);

					if (sessionImplementor.TransactionContext.ShouldCloseSessionOnDistributedTransactionCompleted)
					{
						sessionImplementor.CloseSessionFromDistributedTransaction();
					}
					sessionImplementor.TransactionContext = null;

					// Do not signal it is finished before having processed after-transaction actions, otherwise they
					// may be executed concurrently to next scope, which causes a bunch of issues.
					enlistment.Done();
				}
			}

			#endregion

			public void Dispose()
			{
				if (AmbientTransation != null)
					AmbientTransation.Dispose();
			}
		}
	}
}