using System;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// Manages the database connection and transaction for an <see cref="ISession" />.
	/// </summary>
	/// <remarks>
	/// This class corresponds to ConnectionManager and JDBCContext in Hibernate,
	/// combined.
	/// </remarks>
	[Serializable]
	public class ConnectionManager : ISerializable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ConnectionManager));

		[NonSerialized]
		private IDbConnection connection;
		// Whether we own the connection, i.e. connect and disconnect automatically.
		private bool ownConnection;

		[NonSerialized]
		private ITransaction transaction;

		private readonly ISessionImplementor session;
		private readonly ConnectionReleaseMode connectionReleaseMode;

		[NonSerialized]
		private bool isFlushing;

		public ConnectionManager(
			ISessionImplementor session,
			IDbConnection suppliedConnection,
			ConnectionReleaseMode connectionReleaseMode)
		{
			this.session = session;
			this.connection = suppliedConnection;
			this.connectionReleaseMode = connectionReleaseMode;
			this.ownConnection = suppliedConnection == null;
		}

		public bool IsInActiveTransaction
		{
			get { return transaction != null && transaction.IsActive; }
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
			if (session.Batcher != null)
			{
				session.Batcher.Dispose();
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

			if (session.Batcher != null)
			{
				session.Batcher.CloseCommands();
			}

			CloseConnection();
		}

		public IDbConnection Disconnect()
		{
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
			session.Factory.CloseConnection(connection);
			connection = null;
		}

		public IDbConnection GetConnection()
		{
			if (connection == null)
			{
				if (ownConnection)
				{
					connection = session.Factory.OpenConnection();
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
			else if (IsAggressiveRelease && session.Batcher.HasOpenResources)
			{
				log.Info("forcing batcher resource cleanup on transaction completion; forgot to close ScrollableResults/Enumerable?");
				session.Batcher.CloseCommands();
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
				else if (session.Batcher.HasOpenResources)
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
			if (ownConnection)
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
			this.ownConnection = info.GetBoolean("ownConnection");
			this.session = (ISessionImplementor) info.GetValue("session", typeof(ISessionImplementor));
			this.connectionReleaseMode =
				(ConnectionReleaseMode) info.GetValue("connectionReleaseMode", typeof(ConnectionReleaseMode));
		}

		[SecurityPermission(SecurityAction.LinkDemand,
			Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ownConnection", ownConnection);
			info.AddValue("session", session, typeof(ISessionImplementor));
			info.AddValue("connectionReleaseMode", connectionReleaseMode, typeof(ConnectionReleaseMode));
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
					return connection == null && !session.Batcher.HasOpenResources;
				}
				else
				{
					return connection == null;
				}
			}
		}
	}
}