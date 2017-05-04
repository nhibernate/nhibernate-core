using System;
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
	/// This class corresponds to LogicalConnectionImplementor and JdbcCoordinator in Hibernate,
	/// combined.
	/// </remarks>
	[Serializable]
	public partial class ConnectionManager : ISerializable, IDeserializationCallback
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ConnectionManager));

		public interface Callback
		{
			void ConnectionOpened();
			void ConnectionCleanedUp();
			bool IsTransactionInProgress { get; }
		}

		[NonSerialized]
		private DbConnection connection;
		// Whether we own the connection, i.e. connect and disconnect automatically.
		private bool ownConnection;

		[NonSerialized]
		private ITransaction transaction;

		[NonSerialized]
		private IBatcher batcher;

		private readonly ISessionImplementor session;
		private readonly ConnectionReleaseMode connectionReleaseMode;
		private readonly IInterceptor interceptor;

		[NonSerialized]
		private bool _releasesEnabled = true;

		private bool flushingFromDtcTransaction;

		public ConnectionManager(
			ISessionImplementor session,
			DbConnection suppliedConnection,
			ConnectionReleaseMode connectionReleaseMode,
			IInterceptor interceptor)
		{
			this.session = session;
			connection = suppliedConnection;
			this.connectionReleaseMode = connectionReleaseMode;

			this.interceptor = interceptor;
			batcher = session.Factory.Settings.BatcherFactory.CreateBatcher(this, interceptor);

			ownConnection = suppliedConnection == null;
		}

		public bool IsInActiveTransaction
		{
			get
			{
				if (transaction != null && transaction.IsActive)
					return true;
				return Factory.TransactionFactory.IsInDistributedActiveTransaction(session);
			}
		}

		public bool IsConnected
		{
			get { return connection != null || ownConnection; }
		}

		public void Reconnect()
		{
			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}

			ownConnection = true;
		}

		public void Reconnect(DbConnection suppliedConnection)
		{
			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}

			log.Debug("reconnecting session");
			connection = suppliedConnection;
			ownConnection = false;
		}

		public DbConnection Close()
		{
			if (batcher != null)
			{
				batcher.Dispose();
			}

			if (transaction != null)
			{
				transaction.Dispose();
			}

			// When the connection is null nothing needs to be done - if there
			// is a value for connection then Disconnect() was not called - so we
			// need to ensure it gets called.
			if (connection == null)
			{
				ownConnection = false;
				return null;
			}
			else
			{
				return Disconnect();
			}
		}

		private DbConnection DisconnectSuppliedConnection()
		{
			if (connection == null)
			{
				throw new HibernateException("Session already disconnected");
			}

			var c = connection;
			connection = null;
			return c;
		}

		private void DisconnectOwnConnection()
		{
			if (connection == null)
			{
				// No active connection
				return;
			}

			if (batcher != null)
			{
				batcher.CloseCommands();
			}

			CloseConnection();
		}

		public DbConnection Disconnect()
		{
			if (IsInActiveTransaction)
				throw new InvalidOperationException("Disconnect cannot be called while a transaction is in progress.");

			if (!ownConnection)
			{
				return DisconnectSuppliedConnection();
			}
			else
			{
				DisconnectOwnConnection();
				ownConnection = false;
				return null;
			}
		}

		private void CloseConnection()
		{
			Factory.ConnectionProvider.CloseConnection(connection);
			connection = null;
		}

		public DbConnection GetConnection()
		{
			if (connection == null)
			{
				if (ownConnection)
				{
					connection = Factory.ConnectionProvider.GetConnection();
					if (Factory.Statistics.IsStatisticsEnabled)
					{
						Factory.StatisticsImplementor.Connect();
					}
				}
				else if (session.IsOpen)
				{
					throw new HibernateException("Session is currently disconnected");
				}
				else
				{
					throw new HibernateException("Session is closed");
				}
			}
			return connection;
		}

		public void AfterTransaction()
		{
			if (IsAfterTransactionRelease)
			{
				AggressiveRelease();
			}
			else if (IsAggressiveRelease && batcher.HasOpenResources)
			{
				log.Info("forcing batcher resource cleanup on transaction completion; forgot to close ScrollableResults/Enumerable?");
				batcher.CloseCommands();
				AggressiveRelease();
			}
			else if (IsOnCloseRelease)
			{
				// log a message about potential connection leaks
				log.Debug(
					"transaction completed on session with on_close connection release mode; be sure to close the session to release ADO.Net resources!");
			}
			transaction = null;
		}

		public void AfterStatement()
		{
			if (IsAggressiveRelease)
			{
				if (!_releasesEnabled)
				{
					log.Debug("skipping aggressive-release due to manual disabling");
				}
				else if (batcher.HasOpenResources)
				{
					log.Debug("skipping aggressive-release due to open resources on batcher");
				}
				// TODO H3:
				//else if (borrowedConnection != null)
				//{
				//    log.Debug("skipping aggressive-release due to borrowed connection");
				//}
				else
				{
					AggressiveRelease();
				}
			}
		}

		private void AggressiveRelease()
		{
			if (ownConnection && flushingFromDtcTransaction == false)
			{
				log.Debug("aggressively releasing database connection");
				if (connection != null)
				{
					CloseConnection();
				}
			}
		}

		[NonSerialized]
		private int _flushDepth;

		public void FlushBeginning()
		{
			if (_flushDepth == 0)
			{
				log.Debug("registering flush begin");
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
				log.Debug("registering flush end");
			}
			AfterStatement();
		}

		#region Serialization

		private ConnectionManager(SerializationInfo info, StreamingContext context)
		{
			ownConnection = info.GetBoolean("ownConnection");
			session = (ISessionImplementor)info.GetValue("session", typeof(ISessionImplementor));
			connectionReleaseMode =
				(ConnectionReleaseMode)info.GetValue("connectionReleaseMode", typeof(ConnectionReleaseMode));
			interceptor = (IInterceptor)info.GetValue("interceptor", typeof(IInterceptor));
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ownConnection", ownConnection);
			info.AddValue("session", session, typeof(ISessionImplementor));
			info.AddValue("connectionReleaseMode", connectionReleaseMode, typeof(ConnectionReleaseMode));
			info.AddValue("interceptor", interceptor, typeof(IInterceptor));
		}

		#endregion

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			batcher = Factory.Settings.BatcherFactory.CreateBatcher(this, interceptor);
		}

		#endregion

		public ITransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			Transaction.Begin(isolationLevel);
			return transaction;
		}

		public ITransaction BeginTransaction()
		{
			Transaction.Begin();
			return transaction;
		}

		public ITransaction Transaction
		{
			get
			{
				if (transaction == null)
				{
					transaction = Factory.TransactionFactory.CreateTransaction(session);
				}
				return transaction;
			}
		}

		public void AfterNonTransactionalQuery(bool success)
		{
			log.Debug("after autocommit");
		}

		private bool IsAfterTransactionRelease
		{
			get { return connectionReleaseMode == ConnectionReleaseMode.AfterTransaction; }
		}

		private bool IsOnCloseRelease
		{
			get { return connectionReleaseMode == ConnectionReleaseMode.OnClose; }
		}

		private bool IsAggressiveRelease
		{
			get
			{
				if (connectionReleaseMode == ConnectionReleaseMode.AfterTransaction)
				{
					return !IsInActiveTransaction;
				}
				return false;
			}
		}

		public ISessionFactoryImplementor Factory
		{
			get { return session.Factory; }
		}

		public bool IsReadyForSerialization
		{
			get
			{
				if (ownConnection)
				{
					return connection == null && !batcher.HasOpenResources;
				}
				else
				{
					return connection == null;
				}
			}
		}

		/// <summary> The batcher managed by this ConnectionManager. </summary>
		public IBatcher Batcher
		{
			get { return batcher; }
		}

		public IDisposable FlushingFromDtcTransaction
		{
			get
			{
				flushingFromDtcTransaction = true;
				return new StopFlushingFromDtcTransaction(this);
			}
		}

		private class StopFlushingFromDtcTransaction : IDisposable
		{
			private readonly ConnectionManager manager;

			public StopFlushingFromDtcTransaction(ConnectionManager manager)
			{
				this.manager = manager;
			}

			public void Dispose()
			{
				manager.flushingFromDtcTransaction = false;
			}
		}

		public DbCommand CreateCommand()
		{
			var result = GetConnection().CreateCommand();
			Transaction.Enlist(result);
			return result;
		}
	}
}
