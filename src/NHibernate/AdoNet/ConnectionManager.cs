using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Security;

using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Manages the database connection and transaction for an <see cref="ISession" />.
	/// </summary>
	/// <remarks>
	/// This class corresponds to <c>LogicalConnectionImplementor</c> and <c>JdbcCoordinator</c>
	/// in Hibernate, combined.
	/// </remarks>
	[Serializable]
	public partial class ConnectionManager : ISerializable, IDeserializationCallback
	{
		private static readonly INHibernateLogger _log = NHibernateLogger.For(typeof(ConnectionManager));

		[NonSerialized]
		private DbConnection _connection;
		[NonSerialized]
		private DbConnection _backupConnection;
		[NonSerialized]
		private System.Transactions.Transaction _currentSystemTransaction;
		[NonSerialized]
		private System.Transactions.Transaction _backupCurrentSystemTransaction;
		// Whether we own the connection, i.e. connect and disconnect automatically.
		private bool _ownConnection;

		[NonSerialized]
		private ITransaction _transaction;

		[NonSerialized]
		private IBatcher _batcher;

		private readonly ConnectionReleaseMode _connectionReleaseMode;
		private readonly IInterceptor _interceptor;
		[NonSerialized]
		private readonly List<ISessionImplementor> _dependentSessions = new List<ISessionImplementor>();

		/// <summary>
		/// The session responsible for the lifecycle of the connection manager.
		/// </summary>
		public ISessionImplementor Session { get; }

		/// <summary>
		/// The sessions using the connection manager of the session responsible for it.
		/// </summary>
		public IReadOnlyCollection<ISessionImplementor> DependentSessions => _dependentSessions;

		[NonSerialized]
		private bool _releasesEnabled = true;

		[NonSerialized]
		private bool _processingFromSystemTransaction;
		[NonSerialized]
		private bool _allowConnectionUsage = true;
		// Do we need to release the current connection instead of yielding it?
		[NonSerialized]
		private bool _connectionReleaseRequired;
		// Do we need to explicitly enlist the current connection before yielding it?
		[NonSerialized]
		private bool _connectionEnlistmentRequired;

		/// <summary>
		/// <see langword="true"/> when the connection manager is being used from system transaction completion events,
		/// <see langword="false"/> otherwise.
		/// </summary>
		public bool ProcessingFromSystemTransaction => _processingFromSystemTransaction;

		public ConnectionManager(
			ISessionImplementor session,
			DbConnection suppliedConnection,
			ConnectionReleaseMode connectionReleaseMode,
			IInterceptor interceptor,
			bool shouldAutoJoinTransaction)
		{
			Session = session;
			_connection = suppliedConnection;
			_connectionReleaseMode = connectionReleaseMode;

			_interceptor = interceptor;
			_batcher = session.Factory.Settings.BatcherFactory.CreateBatcher(this, interceptor);

			_ownConnection = suppliedConnection == null;
			ShouldAutoJoinTransaction = shouldAutoJoinTransaction;
		}

		public void AddDependentSession(ISessionImplementor session)
		{
			_dependentSessions.Add(session);
		}

		public void RemoveDependentSession(ISessionImplementor session)
		{
			_dependentSessions.Remove(session);
		}

		public bool IsInActiveExplicitTransaction
			=> _transaction != null && _transaction.IsActive;

		public bool IsInActiveTransaction
			=> IsInActiveExplicitTransaction || Factory.TransactionFactory.IsInActiveSystemTransaction(Session);

		public bool IsConnected
			=> _connection != null || _ownConnection;

		public bool ShouldAutoJoinTransaction { get; }

		public void Reconnect()
		{
			if (IsConnected)
			{
				throw new HibernateException("Session already connected");
			}

			_ownConnection = true;
		}

		public void Reconnect(DbConnection suppliedConnection)
		{
			if (IsConnected)
			{
				throw new HibernateException("Session already connected");
			}

			_log.Debug("Reconnecting session");
			_connection = suppliedConnection;
			_ownConnection = false;

			// May fail if the supplied connection is enlisted in another transaction, which would be an user
			// error. (Either disable auto join transaction or supply an enlist-able connection.)
			if (_currentSystemTransaction != null)
				_connection.EnlistTransaction(_currentSystemTransaction);
		}

		public DbConnection Close()
		{
			_batcher?.Dispose();

			_transaction?.Dispose();

			if (_backupConnection != null)
			{
				_log.Warn("Backup connection was still defined at time of closing.");
				Factory.ConnectionProvider.CloseConnection(_backupConnection);
				_backupConnection = null;
			}

			// When the connection is null nothing needs to be done - if there
			// is a value for connection then Disconnect() was not called - so we
			// need to ensure it gets called.
			if (_connection == null)
			{
				_ownConnection = false;
				return null;
			}
			return Disconnect();
		}

		private DbConnection DisconnectSuppliedConnection()
		{
			if (_connection == null)
			{
				throw new HibernateException("Session already disconnected");
			}

			var c = _connection;
			_connection = null;
			return c;
		}

		private void DisconnectOwnConnection()
		{
			if (_connection == null)
			{
				// No active connection
				return;
			}

			_batcher?.CloseCommands();

			CloseConnection();
		}

		public DbConnection Disconnect()
		{
			if (IsInActiveExplicitTransaction)
				throw new InvalidOperationException("Disconnect cannot be called while an explicit transaction is in progress.");

			if (!_ownConnection)
			{
				return DisconnectSuppliedConnection();
			}

			DisconnectOwnConnection();
			_ownConnection = false;
			return null;
		}

		private void CloseConnection()
		{
			Factory.ConnectionProvider.CloseConnection(_connection);
			_connection = null;
		}

		public DbConnection GetConnection()
		{
			if (!_allowConnectionUsage)
			{
				throw new HibernateException("Connection usage is currently disallowed");
			}

			if (_connectionReleaseRequired)
			{
				_connectionReleaseRequired = false;
				if (_connection != null)
				{
					_log.Debug("Releasing database connection");
					CloseConnection();
				}
			}

			if (_connectionEnlistmentRequired)
			{
				_connectionEnlistmentRequired = false;
				// No null check on transaction: we need to do it for connection supporting it, and
				// _connectionEnlistmentRequired should not be set if the transaction is null while the
				// connection does not support it.
				_connection?.EnlistTransaction(_currentSystemTransaction);
			}

			if (_connection == null)
			{
				if (_ownConnection)
				{
					_connection = Factory.ConnectionProvider.GetConnection();
					//NH-3724
					if (Factory.Settings.NotificationHandler != null)
					{
						Factory.ConnectionProvider.Driver.AddNotificationHandler(_connection, Factory.Settings.NotificationHandler);
					}
					// Will fail if the connection is already enlisted in another transaction.
					// Probable case: nested transaction scope with connection auto-enlistment enabled.
					// That is an user error.
					if (_currentSystemTransaction != null)
						_connection.EnlistTransaction(_currentSystemTransaction);

					if (Factory.Statistics.IsStatisticsEnabled)
					{
						Factory.StatisticsImplementor.Connect();
					}
				}
				else if (Session.IsOpen)
				{
					throw new HibernateException("Session is currently disconnected");
				}
				else
				{
					throw new HibernateException("Session is closed");
				}
			}
			return _connection;
		}

		public void AfterTransaction()
		{
			if (IsAfterTransactionRelease)
			{
				AggressiveRelease();
			}
			else if (IsAggressiveRelease && _batcher.HasOpenResources)
			{
				_log.Info("Forcing batcher resource cleanup on transaction completion; forgot to close ScrollableResults/Enumerable?");
				_batcher.CloseCommands();
				AggressiveRelease();
			}
			else if (IsOnCloseRelease)
			{
				// _log a message about potential connection leaks
				_log.Debug(
					"Transaction completed on session with on_close connection release mode; be sure to close the session to release ADO.Net resources!");
			}
			_transaction = null;
		}

		public void AfterStatement()
		{
			if (IsAggressiveRelease)
			{
				if (!_releasesEnabled)
				{
					_log.Debug("Skipping aggressive-release due to manual disabling");
				}
				else if (_batcher.HasOpenResources)
				{
					_log.Debug("Skipping aggressive-release due to open resources on batcher");
				}
				// TODO H3:
				//else if (borrowedConnection != null)
				//{
				//    _log.Debug("skipping aggressive-release due to borrowed connection");
				//}
				else
				{
					AggressiveRelease();
				}
			}
		}

		private void AggressiveRelease()
		{
			if (_ownConnection)
			{
				if (_connection != null)
				{
					if (_processingFromSystemTransaction)
						_connectionReleaseRequired = true;
					else
					{
						_log.Debug("Aggressively releasing database connection");
						CloseConnection();
					}
				}
			}
		}

		[NonSerialized]
		private int _flushDepth;

		public void FlushBeginning()
		{
			if (_flushDepth == 0)
			{
				_log.Debug("Registering flush begin");
				_releasesEnabled = false;
			}
			_flushDepth++;
		}

		public void FlushEnding()
		{
			_flushDepth--;
			if (_flushDepth < 0)
			{
				throw new HibernateException("Mismatched flush handling");
			}
			if (_flushDepth == 0)
			{
				_releasesEnabled = true;
				_log.Debug("Registering flush end");
			}
			AfterStatement();
		}

		#region Serialization

		private ConnectionManager(SerializationInfo info, StreamingContext context)
		{
			_ownConnection = info.GetBoolean("ownConnection");
			Session = (ISessionImplementor)info.GetValue("session", typeof(ISessionImplementor));
			_connectionReleaseMode =
				(ConnectionReleaseMode)info.GetValue("connectionReleaseMode", typeof(ConnectionReleaseMode));
			_interceptor = (IInterceptor)info.GetValue("interceptor", typeof(IInterceptor));
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ownConnection", _ownConnection);
			info.AddValue("session", Session, typeof(ISessionImplementor));
			info.AddValue("connectionReleaseMode", _connectionReleaseMode, typeof(ConnectionReleaseMode));
			info.AddValue("interceptor", _interceptor, typeof(IInterceptor));
		}

		#endregion

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			_batcher = Factory.Settings.BatcherFactory.CreateBatcher(this, _interceptor);
		}

		#endregion

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			EnsureTransactionIsCreated();
			_transaction.Begin(isolationLevel);
			return _transaction;
		}

		public ITransaction BeginTransaction()
		{
			EnsureTransactionIsCreated();
			_transaction.Begin();
			return _transaction;
		}

		private void EnsureTransactionIsCreated()
		{
			if (_transaction == null)
			{
				_transaction = Factory.TransactionFactory.CreateTransaction(Session);
			}
		}

		// Since v5.3
		[Obsolete("Use CurrentTransaction instead, and check for null.")]
		public ITransaction Transaction
		{
			get
			{
				EnsureTransactionIsCreated();
				return _transaction;
			}
		}

		/// <summary>
		/// The current transaction if any is ongoing, else <see langword="null" />.
		/// </summary>
		public ITransaction CurrentTransaction => _transaction;

		public void AfterNonTransactionalQuery(bool success)
		{
			_log.Debug("After autocommit");
		}

		private bool IsAfterTransactionRelease
			=> _connectionReleaseMode == ConnectionReleaseMode.AfterTransaction;

		private bool IsOnCloseRelease
			=> _connectionReleaseMode == ConnectionReleaseMode.OnClose;

		private bool IsAggressiveRelease
			=> _connectionReleaseMode == ConnectionReleaseMode.AfterTransaction && !IsInActiveTransaction;

		public ISessionFactoryImplementor Factory
			=> Session.Factory;

		public bool IsReadyForSerialization
		{
			get
			{
				if (_ownConnection)
				{
					return _connection == null && !_batcher.HasOpenResources;
				}
				return _connection == null;
			}
		}

		/// <summary> The batcher managed by this ConnectionManager. </summary>
		public IBatcher Batcher
			=> _batcher;

		public DbCommand CreateCommand()
		{
			var result = GetConnection().CreateCommand();
			EnlistInTransaction(result);
			return result;
		}

		/// <summary>
		/// Enlist a command in the current transaction, if any.
		/// </summary>
		/// <param name="command">The command to enlist.</param>
		public void EnlistInTransaction(DbCommand command)
		{
			if (command == null)
				throw new ArgumentNullException(nameof(command));

			if (_transaction != null)
			{
				_transaction.Enlist(command);
				return;
			}

			if (command.Transaction != null)
			{
				_log.Warn("set a nonnull DbCommand.Transaction to null because the Session had no Transaction");
				command.Transaction = null;
			}
		}

		/// <summary>
		/// Enlist the connection into provided transaction if the connection should be enlisted.
		/// Do nothing in case an explicit transaction is ongoing.
		/// </summary>
		/// <param name="transaction">The transaction in which the connection should be enlisted.</param>
		public void EnlistIfRequired(System.Transactions.Transaction transaction)
		{
			if (transaction == _currentSystemTransaction)
			{
				// Short-circuit after having stored the transaction : they may be equal, but not the same reference.
				// And the previous one may be an already disposed dependent clone, in which case we need to update
				// our reference.
				_currentSystemTransaction = transaction;
				return;
			}

			_currentSystemTransaction = transaction;

			// Most connections do not support enlisting in a system transaction while already participating
			// in a local transaction. They are not supposed to be mixed anyway.
			if (IsInActiveExplicitTransaction && transaction != null)
				throw new InvalidOperationException("Cannot enlist in a system transaction while an explicit transaction has been started on the session.");

			if (_connection == null || _connectionReleaseRequired)
				return;

			// Some drivers do not support enlistment with null. Skip for them, they are supposed
			// to un-enlist by themselves.
			if (transaction == null && !Factory.ConnectionProvider.Driver.SupportsNullEnlistment)
			{
				return;
			}

			if (!_allowConnectionUsage)
			{
				_connectionEnlistmentRequired = true;
				return;
			}

			_connection.EnlistTransaction(transaction);
		}

		public IDisposable BeginProcessingFromSystemTransaction(bool allowConnectionUsage)
		{
			var needSwapping = _ownConnection && allowConnectionUsage &&
				Factory.Dialect.SupportsConcurrentWritingConnectionsInSameTransaction;
			if (needSwapping)
			{
				if (Batcher.HasOpenResources)
					throw new InvalidOperationException("Batcher still has opened ressources at time of processing from system transaction.");
				// Swap out current connection for avoiding using it concurrently to its own 2PC
				_backupConnection = _connection;
				_backupCurrentSystemTransaction = _currentSystemTransaction;
				_connection = null;
				_currentSystemTransaction = null;
			}
			_processingFromSystemTransaction = true;
			var wasAllowingConnectionUsage = _allowConnectionUsage;
			_allowConnectionUsage = allowConnectionUsage;
			return new EndFlushingFromSystemTransaction(this, needSwapping, wasAllowingConnectionUsage);
		}

		private class EndFlushingFromSystemTransaction : IDisposable
		{
			private readonly ConnectionManager _manager;
			private readonly bool _hasSwappedConnection;
			private readonly bool _wasAllowingConnectionUsage;

			public EndFlushingFromSystemTransaction(ConnectionManager manager, bool hasSwappedConnection, bool wasAllowingConnectionUsage)
			{
				_manager = manager;
				_hasSwappedConnection = hasSwappedConnection;
				_wasAllowingConnectionUsage = wasAllowingConnectionUsage;
			}

			public void Dispose()
			{
				_manager._processingFromSystemTransaction = false;
				_manager._allowConnectionUsage = _wasAllowingConnectionUsage;

				if (!_hasSwappedConnection)
					return;

				// Release the connection potentially acquired for processing from system transaction.
				_manager.DisconnectOwnConnection();
				// Swap back current connection
				_manager._connection = _manager._backupConnection;
				_manager._currentSystemTransaction = _manager._backupCurrentSystemTransaction;
				_manager._backupConnection = null;
				_manager._backupCurrentSystemTransaction = null;
			}
		}
	}
}
