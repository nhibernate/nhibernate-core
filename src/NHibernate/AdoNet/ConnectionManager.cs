using System;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using log4net;
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Manages the database connection and transaction for an <see cref="ISession" />.
	/// </summary>
	/// <remarks>
	/// This class corresponds to ConnectionManager and JDBCContext in Hibernate,
	/// combined.
	/// </remarks>
	[Serializable]
	public class ConnectionManager : ISerializable, IDeserializationCallback
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ConnectionManager));

		public interface Callback
		{
			void ConnectionOpened();
			void ConnectionCleanedUp();
			bool IsTransactionInProgress { get; }
		}

		[NonSerialized]
		private IDbConnection connection;
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
		private bool isFlushing;

		private bool flushingFromDtcTransaction;

		public ConnectionManager(
			ISessionImplementor session,
			IDbConnection suppliedConnection,
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
				if (System.Transactions.Transaction.Current != null)
					return true;
				return transaction != null && transaction.IsActive;
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

		public void Reconnect(IDbConnection suppliedConnection)
		{
			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}

			log.Debug("reconnecting session");
			connection = suppliedConnection;
			ownConnection = false;
		}

		public IDbConnection Close()
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

		private IDbConnection DisconnectSuppliedConnection()
		{
			if (connection == null)
			{
				throw new HibernateException("Session already disconnected");
			}

			IDbConnection c = connection;
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

		public IDbConnection Disconnect() {
            if (IsInActiveTransaction)
                throw  new InvalidOperationException("Disconnect cannot be called while a transaction is in progress.");
			try
			{
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
			finally
			{
				// Ensure that AfterTransactionCompletion gets called since
				// it takes care of the locks and cache.
				if (!IsInActiveTransaction)
				{
					// We don't know the state of the transaction
					session.AfterTransactionCompletion(false, null);
				}
			}
		}

		private void CloseConnection()
		{
			session.Factory.ConnectionProvider.CloseConnection(connection);
			connection = null;
		}

		public IDbConnection GetConnection()
		{
			if (connection == null)
			{
				if (ownConnection)
				{
					connection = session.Factory.ConnectionProvider.GetConnection();
					if (session.Factory.Statistics.IsStatisticsEnabled)
					{
						session.Factory.StatisticsImplementor.Connect();
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
				if (isFlushing)
				{
					log.Debug("skipping aggressive-release due to flush cycle");
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

		public void FlushBeginning()
		{
			log.Debug("registering flush begin");
			isFlushing = true;
		}

		public void FlushEnding()
		{
			log.Debug("registering flush end");
			isFlushing = false;
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

		[SecurityPermission(SecurityAction.LinkDemand,
			Flags = SecurityPermissionFlag.SerializationFormatter)]
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
			batcher = session.Factory.Settings.BatcherFactory.CreateBatcher(this, interceptor);
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
					transaction = session.Factory.TransactionFactory.CreateTransaction(session);
				}
				return transaction;
			}
		}

		public void AfterNonTransactionalQuery(bool success)
		{
			log.Debug("after autocommit");
			session.AfterTransactionCompletion(success, null);
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
	}
}
