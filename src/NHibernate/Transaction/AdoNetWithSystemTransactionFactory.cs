using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;
using NHibernate.AdoNet;
using NHibernate.Engine;
using NHibernate.Engine.Transaction;
using NHibernate.Util;

namespace NHibernate.Transaction
{
	/// <summary>
	/// <see cref="ITransaction"/> factory implementation supporting system
	/// <see cref="System.Transactions.Transaction"/>.
	/// </summary>
	public partial class AdoNetWithSystemTransactionFactory : AdoNetTransactionFactory
	{
		private static readonly INHibernateLogger _logger = NHibernateLogger.For(typeof(ITransactionFactory));

		/// <summary>
		/// See <see cref="Cfg.Environment.SystemTransactionCompletionLockTimeout"/>.
		/// </summary>
		protected int SystemTransactionCompletionLockTimeout { get; private set; }
		/// <summary>
		/// See <see cref="Cfg.Environment.UseConnectionOnSystemTransactionPrepare"/>.
		/// </summary>
		protected bool UseConnectionOnSystemTransactionPrepare { get; private set; }

		/// <inheritdoc />
		public override void Configure(IDictionary<string, string> props)
		{
			base.Configure(props);
			SystemTransactionCompletionLockTimeout =
				PropertiesHelper.GetInt32(Cfg.Environment.SystemTransactionCompletionLockTimeout, props, 5000);
			if (SystemTransactionCompletionLockTimeout < -1)
				throw new HibernateException(
					$"Invalid {Cfg.Environment.SystemTransactionCompletionLockTimeout} value: {SystemTransactionCompletionLockTimeout}. It can not be less than -1.");
			UseConnectionOnSystemTransactionPrepare =
				PropertiesHelper.GetBoolean(Cfg.Environment.UseConnectionOnSystemTransactionPrepare, props, true);
		}

		/// <inheritdoc />
		public override void EnlistInSystemTransactionIfNeeded(ISessionImplementor session)
		{
			if (session == null)
				throw new ArgumentNullException(nameof(session));

			if (!session.ConnectionManager.ShouldAutoJoinTransaction)
			{
				return;
			}

			JoinSystemTransaction(session, System.Transactions.Transaction.Current);
		}

		/// <inheritdoc />
		public override void ExplicitJoinSystemTransaction(ISessionImplementor session)
		{
			if (session == null)
				throw new ArgumentNullException(nameof(session));
			var transaction = System.Transactions.Transaction.Current;
			if (transaction == null)
				throw new HibernateException("No current system transaction to join.");

			JoinSystemTransaction(session, transaction);
		}

		/// <summary>
		/// Enlist the session in the supplied transaction.
		/// </summary>
		/// <param name="session">The session to enlist.</param>
		/// <param name="transaction">The transaction to enlist with. Can be <see langword="null"/>.</param>
		protected virtual void JoinSystemTransaction(ISessionImplementor session, System.Transactions.Transaction transaction)
		{
			// Handle the transaction on the originating session only.
			var originatingSession = session.ConnectionManager.Session;

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
					EnlistDependentSession(session, originatingSession.TransactionContext);
				}
				return;
			}

			var transactionContext = CreateAndEnlistMainContext(originatingSession, transaction);
			originatingSession.TransactionContext = transactionContext;

			_logger.Debug(
				"Enlisted into system transaction: {0}",
				transaction.IsolationLevel);

			originatingSession.AfterTransactionBegin(null);
			foreach (var dependentSession in originatingSession.ConnectionManager.DependentSessions)
			{
				EnlistDependentSession(dependentSession, transactionContext);
			}
		}

		/// <summary>
		/// Create a transaction context for enlisting a session with a <see cref="System.Transactions.Transaction"/>,
		/// and enlist the context in the transaction.
		/// </summary>
		/// <param name="session">The session to be enlisted.</param>
		/// <param name="transaction">The transaction into which the context has to be enlisted.</param>
		/// <returns>The created transaction context.</returns>
		protected virtual ITransactionContext CreateAndEnlistMainContext(
			ISessionImplementor session,
			System.Transactions.Transaction transaction)
		{
			var transactionContext = new SystemTransactionContext(
				session, transaction, SystemTransactionCompletionLockTimeout,
				UseConnectionOnSystemTransactionPrepare);
			transactionContext.EnlistedTransaction.EnlistVolatile(
				transactionContext,
				UseConnectionOnSystemTransactionPrepare
					? EnlistmentOptions.EnlistDuringPrepareRequired
					: EnlistmentOptions.None);
			return transactionContext;
		}

		private void EnlistDependentSession(ISessionImplementor dependentSession, ITransactionContext mainContext)
		{
			dependentSession.TransactionContext = CreateDependentContext(dependentSession, mainContext);
			dependentSession.AfterTransactionBegin(null);
		}

		/// <summary>
		/// Create a transaction context for a dependent session.
		/// </summary>
		/// <param name="dependentSession">The dependent session.</param>
		/// <param name="mainContext">The context of the session owning the <see cref="ConnectionManager"/>.</param>
		/// <returns>A dependent context for the session.</returns>
		protected virtual ITransactionContext CreateDependentContext(ISessionImplementor dependentSession, ITransactionContext mainContext)
		{
			return new DependentContext(mainContext);
		}

		/// <inheritdoc />
		public override bool IsInActiveSystemTransaction(ISessionImplementor session)
			=> session?.TransactionContext?.IsInActiveTransaction ?? false;

		/// <inheritdoc />
		public override void ExecuteWorkInIsolation(ISessionImplementor session, IIsolatedWork work, bool transacted)
		{
			using (var tx = new TransactionScope(TransactionScopeOption.Suppress))
			{
				base.ExecuteWorkInIsolation(session, work, transacted);
				tx.Complete();
			}
		}

		/// <summary>
		/// Transaction context for enlisting a session with a system <see cref="System.Transactions.Transaction"/>.
		/// It is meant for being the concrete class enlisted in the transaction.
		/// </summary>
		public class SystemTransactionContext : ITransactionContext, IEnlistmentNotification
		{
			/// <summary>
			/// The transaction in which this context is enlisted.
			/// </summary>
			protected internal System.Transactions.Transaction EnlistedTransaction { get; }
			/// <inheritdoc />
			public bool ShouldCloseSessionOnSystemTransactionCompleted { get; set; }
			/// <inheritdoc />
			public bool IsInActiveTransaction { get; protected set; } = true;
			/// <inheritdoc />
			public virtual bool CanFlushOnSystemTransactionCompleted => _useConnectionOnSystemTransactionPrepare;

			private readonly ISessionImplementor _session;
			private readonly bool _useConnectionOnSystemTransactionPrepare;
			private readonly System.Transactions.Transaction _originalTransaction;
			private readonly ManualResetEventSlim _lock = new ManualResetEventSlim(true);
			private volatile bool _needCompletionLocking = true;
			private bool _preparing;
			// Required for not locking the completion phase itself when locking session usages from concurrent threads.
			private static readonly AsyncLocal<bool> _bypassLock = new AsyncLocal<bool>();
			private readonly int _systemTransactionCompletionLockTimeout;

			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="session">The session to enlist with the transaction.</param>
			/// <param name="transaction">The transaction into which the context will be enlisted.</param>
			/// <param name="systemTransactionCompletionLockTimeout">See <see cref="Cfg.Environment.SystemTransactionCompletionLockTimeout"/>.</param>
			/// <param name="useConnectionOnSystemTransactionPrepare">See <see cref="Cfg.Environment.UseConnectionOnSystemTransactionPrepare"/>.</param>
			public SystemTransactionContext(
				ISessionImplementor session,
				System.Transactions.Transaction transaction,
				int systemTransactionCompletionLockTimeout,
				bool useConnectionOnSystemTransactionPrepare)
			{
				_session = session ?? throw new ArgumentNullException(nameof(session));
				_originalTransaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
				EnlistedTransaction = transaction.Clone();
				_systemTransactionCompletionLockTimeout = systemTransactionCompletionLockTimeout;
				_useConnectionOnSystemTransactionPrepare = useConnectionOnSystemTransactionPrepare;
			}

			/// <inheritdoc />
			public virtual void Wait()
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
					// the scope disposal is supposed to block until the transaction has completed. When not
					// distributed, all is done, no wait. When distributed, with MSDTC, the scope disposal is
					// left after all prepare phases, and the complete of all resources including the NHibernate
					// one is concurrently raised. So the wait should indeed only have to wait after NHibernate
					// AfterTransaction events.
					// Remove the block then throw.
					Unlock();
					throw new HibernateException(
						$"Synchronization timeout for transaction completion. Either raise" +
						$"{Cfg.Environment.SystemTransactionCompletionLockTimeout}, or check all scopes are properly" +
						$"disposed and/or all direct System.Transaction.Current changes are properly managed.");
				}
				catch (HibernateException)
				{
					throw;
				}
				catch (Exception ex)
				{
					_logger.Warn(
						ex,
						"Synchronization failure, assuming it has been concurrently disposed and does not need sync anymore.");
				}
			}

			/// <summary>
			/// Lock the context, causing <see cref="Wait"/> to block until released. Do nothing if the context
			/// has already been locked once.
			/// </summary>
			protected virtual void Lock()
			{
				if (!_needCompletionLocking || _isDisposed)
					return;
				_needCompletionLocking = false;
				_lock.Reset();
			}

			/// <summary>
			/// Unlock the context, causing <see cref="Wait"/> to cease blocking. Do nothing if the context
			/// is not locked.
			/// </summary>
			protected virtual void Unlock()
			{
				_lock.Set();
				_bypassLock.Value = false;
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
					// Cloned transaction is not disposed "unexpectedly", its status is accessible till context disposal.
					var status = EnlistedTransaction.TransactionInformation.Status;
					if (status != TransactionStatus.Active || _preparing)
						return status;

					// The clone status can be out of date when active and not in prepare phase, in case of rollback or
					// dependent clone usage.
					// In such case the original transaction is already disposed, and trying to check its status will
					// trigger a dispose exception.
					return _originalTransaction.TransactionInformation.Status;
				}
				catch (ObjectDisposedException ode)
				{
					// For ruling out the dependent clone case when possible, we check if the current transaction is
					// equal to the context one (System.Transactions.Transaction does override equality for this), and
					// in such case, we check the state of the current transaction instead. (The state of the current
					// transaction if equal can only be the same, but it will be inaccessible in case of rollback, due
					// to the current transaction being already disposed.)
					// The current transaction may not be reachable during 2PC phases and transaction completion events,
					// but in such cases the context transaction is either no more active or in prepare phase, which is
					// already covered by _preparing test.
					try
					{
						var currentTransaction = System.Transactions.Transaction.Current;
						if (!ReferenceEquals(currentTransaction, _originalTransaction) &&
							currentTransaction == EnlistedTransaction)
							return currentTransaction.TransactionInformation.Status;
					}
					catch (ObjectDisposedException)
					{
						// Just ignore that one, no use to log two dispose exceptions which are indeed the same.
					}
					catch (InvalidOperationException ioe)
					{
						_logger.Warn(ioe, "Attempting to dodge a disposed transaction trouble, current" +
						             "transaction was unreachable.");
					}

					_logger.Warn(ode, "Enlisted transaction status is maybe wrongly active, original " +
					             "transaction being already disposed. Will assume neither active nor committed.");
					return null;
				}
			}

			#region IEnlistmentNotification Members

			/// <summary>
			/// Prepare the session for the transaction commit. Run
			/// <see cref="ISessionImplementor.BeforeTransactionCompletion(ITransaction)"/> for the session and for
			/// <see cref="ConnectionManager.DependentSessions"/> if any. <see cref="Lock"/> the context
			/// before signaling it is done, or before rollback in case of failure.
			/// </summary>
			/// <param name="preparingEnlistment">The object for notifying the prepare phase outcome.</param>
			public virtual void Prepare(PreparingEnlistment preparingEnlistment)
			{
				_preparing = true;
				using (_session.BeginContext())
				{
					try
					{
						using (_session.ConnectionManager.BeginProcessingFromSystemTransaction(_useConnectionOnSystemTransactionPrepare))
						{
							if (_useConnectionOnSystemTransactionPrepare)
							{
								// Ensure any newly acquired connection gets enlisted in the transaction. When distributed,
								// this code runs from another thread and we cannot rely on Transaction.Current.
								using (var tx = new TransactionScope(EnlistedTransaction))
								{
									// Required when both connection auto-enlistment and session auto-enlistment are disabled.
									_session.JoinTransaction();
									_session.BeforeTransactionCompletion(null);
									foreach (var dependentSession in _session.ConnectionManager.DependentSessions)
										dependentSession.BeforeTransactionCompletion(null);

									tx.Complete();
								}
							}
							else
							{
								_session.BeforeTransactionCompletion(null);
								foreach (var dependentSession in _session.ConnectionManager.DependentSessions)
									dependentSession.BeforeTransactionCompletion(null);
							}
						}
						// Lock the session to ensure second phase gets done before the session is used by code following
						// the transaction scope disposal.
						Lock();

						_logger.Debug("Prepared for system transaction");
						_preparing = false;
						preparingEnlistment.Prepared();
					}
					catch (Exception exception)
					{
						_preparing = false;
						_logger.Error(exception, "System transaction prepare phase failed");
						try
						{
							CompleteTransaction(false);
						}
						finally
						{
							preparingEnlistment.ForceRollback(exception);
						}
					}
				}
			}

			void IEnlistmentNotification.Commit(Enlistment enlistment)
				=> ProcessSecondPhase(enlistment, true);

			// May be called in case of scope disposal without being completed, on transaction timeout or other failure like
			// deadlocks. Not called in case of ForceRollback from the prepare phase of this enlistment.
			void IEnlistmentNotification.Rollback(Enlistment enlistment)
				=> ProcessSecondPhase(enlistment, false);

			void IEnlistmentNotification.InDoubt(Enlistment enlistment)
				=> ProcessSecondPhase(enlistment, null);

			/// <summary>
			/// Handle the second phase callbacks. Has no actual work to do excepted signaling it is done.
			/// </summary>
			/// <param name="enlistment">The enlistment object for signaling to the transaction manager the notification has been handled.</param>
			/// <param name="success"><see langword="true"/> if this is a commit callback, <see langword="false"/> if this is a rollback
			/// callback, <see langword="null"/> if this is an in-doubt callback.</param>
			protected virtual void ProcessSecondPhase(Enlistment enlistment, bool? success)
			{
				using (_session.BeginContext())
				{
					_logger.Debug(
						success.HasValue
							? success.Value
								? "Committing system transaction"
								: "Rolled back system transaction"
							: "System transaction is in doubt");

					try
					{
						CompleteTransaction(success ?? false);
					}
					finally
					{
						enlistment.Done();
					}
				}
			}

			#endregion

			/// <summary>
			/// Handle the transaction completion. Notify <see cref="ConnectionManager"/> of the end of the
			/// transaction. Notify end of transaction to the session and to <see cref="ConnectionManager.DependentSessions"/>
			/// if any. Close sessions requiring it then cleanup transaction contexts and then <see cref="Unlock"/> blocked
			/// threads.
			/// </summary>
			/// <param name="isCommitted"><see langword="true"/> if the transaction is committed, <see langword="false"/>
			/// otherwise.</param>
			protected virtual void CompleteTransaction(bool isCommitted)
			{
				try
				{
					// Allow transaction completed actions to run while others stay blocked.
					_bypassLock.Value = true;
					using (_session.BeginContext())
					{
						// Flag active as false before running actions, otherwise the session may not cleanup as much
						// as possible.
						IsInActiveTransaction = false;
						// Never allows using connection on after transaction event. And tell the connection manager
						// it is called from system transaction. Allows releasing of connection on next usage
						// when release mode is on commit, allows un-enlisting the connection on next usage
						// when release mode is on close. Without BeginsProcessingFromSystemTransaction(false),
						// the connection manager would attempt those operations immediately, causing concurrency
						// issues and crashes for some data providers.
						using (_session.ConnectionManager.BeginProcessingFromSystemTransaction(false))
						{
							_session.ConnectionManager.AfterTransaction();
							// Required for un-enlisting the connection manager when auto-join is false.
							// Not done in AfterTransaction, because users may use NHibernate transactions
							// within scopes, although mixing is not advised.
							if (!ShouldCloseSessionOnSystemTransactionCompleted)
								_session.ConnectionManager.EnlistIfRequired(null);
							
							_session.AfterTransactionCompletion(isCommitted, null);
							foreach (var dependentSession in _session.ConnectionManager.DependentSessions)
								dependentSession.AfterTransactionCompletion(isCommitted, null);

							Cleanup(_session);
						}
					}
				}
				catch (Exception ex)
				{
					// May be run in a dedicated thread. Log any error, otherwise they could stay unlogged.
					_logger.Error(ex, "Failure at transaction completion");
					throw;
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
					// Do not nullify TransactionContext here, could create a race condition with
					// would be await-er on session for disposal (test cases cleanup checks by example).
					if (dependentContext == null)
						continue;
					// Race condition with session disposal is protected on session side by Wait.
					if (dependentContext.ShouldCloseSessionOnSystemTransactionCompleted)
						// This changes the enumerated collection.
						dependentSession.CloseSessionFromSystemTransaction();
					// Now we can (and even must) nullify it.
					dependentSession.TransactionContext = null;
					dependentContext.Dispose();
				}
				var context = session.TransactionContext;
				// Do not nullify TransactionContext here, could create a race condition with
				// would be await-er on session for disposal (test cases cleanup checks by example).
				// Race condition with session disposal is protected on session side by Wait.
				if (context.ShouldCloseSessionOnSystemTransactionCompleted)
				{
					// This closes the connection manager, which will release the connection.
					// This can cause issues with the connection own second phase and concurrency issues
					// when the transaction is distributed. In such case, user needs to disable
					// UseConnectionOnSystemTransactionPrepare.
					session.CloseSessionFromSystemTransaction();
				}
				// Now we can (and even must) nullify it.
				session.TransactionContext = null;
				// No context dispose, done later.
			}

			private bool _isDisposed;

			/// <inheritdoc />
			public void Dispose()
			{
				if (_isDisposed)
					// Avoid disposing twice.
					return;
				_isDisposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Dispose of the context.
			/// </summary>
			/// <param name="disposing"><see langword="true" /> if called by <see cref="Dispose()"/>.
			/// <see langword="false" /> otherwise. Do not access managed resources if it is
			/// <c>false</c>.</param>
			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					Unlock();
					EnlistedTransaction.Dispose();
					_lock.Dispose();
				}
			}
		}

		/// <summary>
		/// Transaction context for enlisting a dependent session. Dependent sessions are not owning
		/// their <see cref="ConnectionManager"/>. The session owning it will have a transaction context
		/// handling all actions for dependent sessions.
		/// </summary>
		public class DependentContext : ITransactionContext
		{
			/// <inheritdoc />
			public bool IsInActiveTransaction
				=> MainTransactionContext.IsInActiveTransaction;

			/// <inheritdoc />
			public bool ShouldCloseSessionOnSystemTransactionCompleted { get; set; }

			/// <inheritdoc />
			public virtual bool CanFlushOnSystemTransactionCompleted
				=> MainTransactionContext.CanFlushOnSystemTransactionCompleted;

			/// <summary>
			/// The transaction context of the session owning the <see cref="ConnectionManager"/>.
			/// </summary>
			protected ITransactionContext MainTransactionContext { get; }

			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="mainTransactionContext">The transaction context of the session owning the
			/// <see cref="ConnectionManager"/>.</param>
			public DependentContext(ITransactionContext mainTransactionContext)
			{
				MainTransactionContext = mainTransactionContext ?? throw new ArgumentNullException(nameof(mainTransactionContext));
			}

			/// <inheritdoc />
			public virtual void Wait() =>
				MainTransactionContext.Wait();

			private bool _isDisposed;

			/// <inheritdoc />
			public void Dispose()
			{
				if (_isDisposed)
					// Avoid disposing twice.
					return;
				_isDisposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Dispose of the context.
			/// </summary>
			/// <param name="disposing"><see langword="true" /> if called by <see cref="Dispose()"/>.
			/// <see langword="false" /> otherwise. Do not access managed resources if it is
			/// <c>false</c>.</param>
			protected virtual void Dispose(bool disposing)
			{
			}
		}
	}
}
