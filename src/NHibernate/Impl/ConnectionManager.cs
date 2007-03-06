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
		private bool connectForNextOperation;

		[NonSerialized]
		private IDbConnection connection;

		[NonSerialized]
		private ITransaction transaction;

		private readonly ISessionImplementor session;
		private bool autoClose;
		private readonly ConnectionReleaseMode connectionReleaseMode;

		[NonSerialized]
		private bool isFlushing;

		public ConnectionManager(
			ISessionImplementor session,
			IDbConnection connection,
			ConnectionReleaseMode connectionReleaseMode)
		{
			this.session = session;
			this.connection = connection;
			this.connectForNextOperation = connection == null;
			this.connectionReleaseMode = connectionReleaseMode;
			this.autoClose = connection == null;
		}

		public void Connect()
		{
			connection = session.Factory.OpenConnection();
			connectForNextOperation = false;
		}

		public bool IsInActiveTransaction
		{
			get { return transaction != null && transaction.IsActive; }
		}

		public bool IsConnected
		{
			get { return connection != null || connectForNextOperation; }
		}

		public void Reconnect()
		{
			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}

			connectForNextOperation = true;
			autoClose = true;
		}

		public void Reconnect(IDbConnection conn)
		{
			if (IsConnected)
			{
				throw new HibernateException("session already connected");
			}

			log.Debug("reconnecting session");
			connection = conn;
			autoClose = false;
		}

		public IDbConnection Close()
		{
			// When the connection is null nothing needs to be done - if there
			// is a value for connection then Disconnect() was not called - so we
			// need to ensure it gets called.
			if (connection == null)
			{
				connectForNextOperation = false;
				return null;
			}
			else
			{
				return Disconnect();
			}
		}

		public IDbConnection Disconnect()
		{
			try
			{
				if (connectForNextOperation)
				{
					connectForNextOperation = false;
					return null;
				}
				else if (connection == null)
				{
					throw new HibernateException("Session already disconnected");
				}
				else
				{
					if (session.Batcher != null)
					{
						session.Batcher.CloseCommands();
					}

					// Get a new reference to the the connection before closing
					// it - and set the existing connection to null but don't
					// close it yet.
					IDbConnection c = connection;

					if (autoClose)
					{
						// Let the SessionFactory close it and return null
						// because the connection is internal to the Session
						CloseConnection();
						return null;
					}
					else
					{
						// Return the connection the user provided - at this point
						// it has been dissociated from the NHibernate session.
						connection = null;
						return c;
					}
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

		// This method does not implement IDisposable.Dispose entirely correctly
		// so the class is not declared to implement IDisposable.
		public void Dispose()
		{
			if (transaction != null)
			{
				transaction.Dispose();
			}

			// we are not reusing the Close() method because that sets the connection==null
			// during the Close() - if the connection is null we can't get to it to Dispose
			// of it.
			if (connection != null)
			{
				if (connection.State == ConnectionState.Closed)
				{
					log.Warn("finalizing unclosed session with closed connection");
				}
				else
				{
					// if the Session is responsible for managing the connection then make sure
					// the connection is disposed of.
					if (autoClose)
					{
						CloseConnection();
					}
				}
			}
		}

		private void CloseConnection()
		{
			session.Factory.CloseConnection(connection);
			connection = null;
			connectForNextOperation = true;
		}

		public IDbConnection GetConnection()
		{
			if (connection == null)
			{
				if (connectForNextOperation)
				{
					Connect();
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
			if (autoClose)
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
			this.connectForNextOperation = false;
			this.autoClose = info.GetBoolean("autoClose");
			this.session = (ISessionImplementor) info.GetValue("session", typeof(ISessionImplementor));
			this.connectionReleaseMode =
				(ConnectionReleaseMode) info.GetValue("connectionReleaseMode", typeof(ConnectionReleaseMode));
		}

		[SecurityPermission(SecurityAction.LinkDemand,
			Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("autoClose", autoClose);
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
				if (autoClose)
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