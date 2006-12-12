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
		private static readonly ILog log = LogManager.GetLogger(typeof (ConnectionManager));

		[NonSerialized]
		private bool connectForNextOperation;

		[NonSerialized]
		private IDbConnection connection;

		[NonSerialized]
		private ITransaction transaction;

		private readonly ISessionImplementor session;
		private bool autoClose;

		public ConnectionManager(ISessionImplementor session, IDbConnection connection, bool autoClose)
		{
			this.session = session;
			this.connection = connection;
			this.connectForNextOperation = connection == null;
			this.autoClose = autoClose;
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
					connection = null;

					if (autoClose)
					{
						// Let the SessionFactory close it and return null
						// because the connection is internal to the Session
						session.Factory.CloseConnection(c);
						return null;
					}
					else
					{
						// Return the connection the user provided - at this point
						// it has been dissociated from the NHibernate session. 
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
					session.AfterTransactionCompletion(false);
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
						session.Factory.CloseConnection(connection);
					}
				}
			}
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

		public void AfterTransactionCompletion()
		{
			transaction = null;
		}

		#region Serialization

		private ConnectionManager(SerializationInfo info, StreamingContext context)
		{
			this.connectForNextOperation = false;
			this.autoClose = info.GetBoolean("autoClose");
			this.session = (ISessionImplementor) info.GetValue("session", typeof (ISessionImplementor));
		}

		[SecurityPermission(SecurityAction.LinkDemand,
			Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("autoClose", autoClose);
			info.AddValue("session", session, typeof(ISessionImplementor));
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
	}
}
