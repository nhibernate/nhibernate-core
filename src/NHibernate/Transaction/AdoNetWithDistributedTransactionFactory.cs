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

			var notification = new PrepareTransactionNotification(session, transaction);

			transaction.EnlistVolatile(
				notification,
				EnlistmentOptions.EnlistDuringPrepareRequired);

			session.TransactionContext = notification;

			logger.DebugFormat("enlisted into DTC transaction: {0}", transaction.IsolationLevel.ToString());
			session.AfterTransactionBegin(null);
		}

		public bool IsInDistributedActiveTransaction(ISessionImplementor session)
		{
			return session.TransactionContext != null;
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

		class PrepareTransactionNotification : IEnlistmentNotification, ITransactionContext
		{
			System.Transactions.Transaction _transaction;

			ISessionImplementor _session;

			public PrepareTransactionNotification(ISessionImplementor session, System.Transactions.Transaction transaction)
			{
				_session = session;
				_transaction = transaction.Clone();
			}

			public bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }

			void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
			{
				using (new SessionIdLoggingContext(_session.SessionId))
				{
					try
					{
						using (var tx = new TransactionScope(_transaction))
						{
							_session.BeforeTransactionCompletion(null);
							if (_session.FlushMode != FlushMode.Never && _session.ConnectionManager.IsConnected)
							{
								using (_session.ConnectionManager.FlushingFromDtcTransaction)
								{
									logger.Debug(
										string.Format(
											"[session-id={0}] Flushing from Dtc Transaction",
											_session.SessionId.ToString()));
									_session.Flush();
								}
							}
							logger.Debug("prepared for DTC transaction");

							tx.Complete();
						}
						preparingEnlistment.Prepared();
					}
					catch (Exception exception)
					{
						preparingEnlistment.ForceRollback(exception);
					}
				}
			}

			void IEnlistmentNotification.Commit(Enlistment enlistment)
			{
				using (this)
				using (new SessionIdLoggingContext(_session.SessionId))
				{
					try
					{
						logger.Debug("Committing DTC transaction");
						OnTransactionCompleted(true);
					}
					catch (Exception e)
					{
						logger.Warn("Exception happened at DTC transaction commit phase", e);
					}
					finally
					{
						enlistment.Done();
					}
				}
			}

			void IEnlistmentNotification.Rollback(Enlistment enlistment)
			{
				using (this)
				using (new SessionIdLoggingContext(_session.SessionId))
				{
					try
					{
						logger.Debug("Rolled back DTC transaction");
						OnTransactionCompleted(false);
					}
					catch (Exception e)
					{
						logger.Warn("Exception happened at DTC transaction rollback phase", e);
					}
					finally
					{
						enlistment.Done();
					}
				}
			}

			void IEnlistmentNotification.InDoubt(Enlistment enlistment)
			{
				using (this)
				using (new SessionIdLoggingContext(_session.SessionId))
				{
					try
					{
						logger.Debug("DTC transaction is in doubt");
						OnTransactionCompleted(false);
					}
					catch (Exception e)
					{
						logger.Warn("Exception happened at DTC transaction in doubt phase", e);
					}
					finally
					{
						enlistment.Done();
					}
				}
			}

			public void Dispose()
			{
				if (!ReferenceEquals(_transaction, null))
					_transaction.Dispose();
				_transaction = null;
				if (!ReferenceEquals(_session, null))
					_session.TransactionContext = null;
				_session = null;
			}

			void OnTransactionCompleted(bool successful)
			{
				var transactionContext = _session.TransactionContext;
				_session.TransactionContext = null;
				_session.AfterTransactionCompletion(successful, null);
				if (transactionContext.ShouldCloseSessionOnDistributedTransactionCompleted)
					_session.CloseSessionFromDistributedTransaction();
			}
		}
	}
}