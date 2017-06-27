using System;
using System.Collections;
using System.Linq;
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
			
			if (System.Transactions.Transaction.Current == null)
				return;

			var originatingSession = session.ConnectionManager.Session;
			if (originatingSession != session)
			{
				session.TransactionContext = new DependentContext();
			}

			if (originatingSession.TransactionContext != null)
				return;

			session = originatingSession;

			var transactionContext = new DistributedTransactionContext(session,
																	   System.Transactions.Transaction.Current);
			session.TransactionContext = transactionContext;
			logger.DebugFormat("enlisted into DTC transaction: {0}",
							   transactionContext.AmbientTransation.IsolationLevel);
			session.AfterTransactionBegin(null);
			foreach (var dependentSession in session.ConnectionManager.DependentSessions)
				dependentSession.AfterTransactionBegin(null);

			TransactionCompletedEventHandler handler = null;

			handler = delegate(object sender, TransactionEventArgs e)
				{
					using (new SessionIdLoggingContext(session.SessionId))
					{
						((DistributedTransactionContext) session.TransactionContext).IsInActiveTransaction = false;

						bool wasSuccessful = false;
						try
						{
							wasSuccessful = e.Transaction.TransactionInformation.Status
											== TransactionStatus.Committed;
						}
						catch (ObjectDisposedException ode)
						{
							logger.Warn("Completed transaction was disposed, assuming transaction rollback", ode);
						}
						session.ConnectionManager.AfterTransaction();
						session.AfterTransactionCompletion(wasSuccessful, null);
						foreach (var dependentSession in session.ConnectionManager.DependentSessions)
							dependentSession.AfterTransactionCompletion(wasSuccessful, null);

						Cleanup(session);
					}

					e.Transaction.TransactionCompleted -= handler;
				};

			transactionContext.AmbientTransation.TransactionCompleted += handler;

			transactionContext.AmbientTransation.EnlistVolatile(transactionContext,
																EnlistmentOptions.EnlistDuringPrepareRequired);
		}

		private static void Cleanup(ISessionImplementor session)
		{
			foreach (var dependentSession in session.ConnectionManager.DependentSessions.ToList())
			{
				if (dependentSession.TransactionContext?.ShouldCloseSessionOnDistributedTransactionCompleted ?? false)
					// This change the enumerated collection.
					dependentSession.CloseSessionFromDistributedTransaction();
				dependentSession.TransactionContext?.Dispose();
				dependentSession.TransactionContext = null;
			}
			if (session.TransactionContext.ShouldCloseSessionOnDistributedTransactionCompleted)
			{
				session.CloseSessionFromDistributedTransaction();
			}
			session.TransactionContext.Dispose();
			session.TransactionContext = null;
		}

		public bool IsInDistributedActiveTransaction(ISessionImplementor session)
		{
			var distributedTransactionContext = (DistributedTransactionContext)session.ConnectionManager.Session.TransactionContext;
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
							if (sessionImplementor.ConnectionManager.IsConnected)
							{
								using (sessionImplementor.ConnectionManager.FlushingFromDtcTransaction)
								{
									sessionImplementor.BeforeTransactionCompletion(null);
									foreach (var dependentSession in sessionImplementor.ConnectionManager.DependentSessions)
										dependentSession.BeforeTransactionCompletion(null);

									logger.Debug("prepared for DTC transaction");

									tx.Complete();
								}
							}
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
			{
				using (new SessionIdLoggingContext(sessionImplementor.SessionId))
				{
					logger.Debug("committing DTC transaction");
					// we have nothing to do here, since it is the actual
					// DB connection that will commit the transaction
					enlistment.Done();
					IsInActiveTransaction = false;
				}
			}

			void IEnlistmentNotification.Rollback(Enlistment enlistment)
			{
				using (new SessionIdLoggingContext(sessionImplementor.SessionId))
				{
					logger.Debug("rolled back DTC transaction");
					// Currently AfterTransactionCompletion is called by the handler for the TransactionCompleted event.
					//sessionImplementor.AfterTransactionCompletion(false, null);
					enlistment.Done();
					IsInActiveTransaction = false;
				}
			}

			void IEnlistmentNotification.InDoubt(Enlistment enlistment)
			{
				using (new SessionIdLoggingContext(sessionImplementor.SessionId))
				{
					sessionImplementor.ConnectionManager.AfterTransaction();
					sessionImplementor.AfterTransactionCompletion(false, null);
					foreach (var dependentSession in sessionImplementor.ConnectionManager.DependentSessions)
						dependentSession.AfterTransactionCompletion(false, null);
					logger.Debug("DTC transaction is in doubt");
					enlistment.Done();
					IsInActiveTransaction = false;
				}
			}

			#endregion

			public void Dispose()
			{
				if (AmbientTransation != null)
					AmbientTransation.Dispose();
			}
		}

		public class DependentContext : ITransactionContext
		{
			public bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }

			public void Dispose() { }
		}
	}
}
