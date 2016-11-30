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

			session.TransactionContext = new DistributedTransactionContext();

			transaction.EnlistVolatile(
				new PrepareTransactionNotification(session, transaction),
				EnlistmentOptions.EnlistDuringPrepareRequired);

			transaction.EnlistVolatile(
				new TransactionCompletionNotification(session),
				EnlistmentOptions.None);

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

		class DistributedTransactionContext : ITransactionContext
		{
			public bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }

			public void Dispose()
			{
			}
		}

		class PrepareTransactionNotification : IEnlistmentNotification
		{
			System.Transactions.Transaction _transaction;

			ISessionImplementor _session;

			public PrepareTransactionNotification(ISessionImplementor session, System.Transactions.Transaction transaction)
			{
				_session = session;
				_transaction = transaction.Clone();
			}

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
					finally
					{
						_session = null;
						try
						{
							if (_transaction != null) _transaction.Dispose();
						}
						finally
						{
							_transaction = null;
						}
					}
				}
			}

			void IEnlistmentNotification.Commit(Enlistment enlistment)
			{
				enlistment.Done();
			}

			void IEnlistmentNotification.Rollback(Enlistment enlistment)
			{
				enlistment.Done();
			}

			void IEnlistmentNotification.InDoubt(Enlistment enlistment)
			{
				enlistment.Done();
			}
		}

		class TransactionCompletionNotification : IEnlistmentNotification
		{
			ISessionImplementor _session;

			public TransactionCompletionNotification(ISessionImplementor session)
			{
				_session = session;
			}

			void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
			{
				preparingEnlistment.Prepared();
			}

			void IEnlistmentNotification.Commit(Enlistment enlistment)
			{
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
						_session = null;
						enlistment.Done();
					}
				}
			}

			void IEnlistmentNotification.Rollback(Enlistment enlistment)
			{
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
						_session = null;
						enlistment.Done();
					}
				}
			}

			void IEnlistmentNotification.InDoubt(Enlistment enlistment)
			{
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
						_session = null;
						enlistment.Done();
					}
				}
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