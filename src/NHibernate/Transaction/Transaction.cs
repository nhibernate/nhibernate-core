using System;
using System.Data;

using NHibernate.Engine;
namespace NHibernate.Transaction 
{
	/// <summary>
	/// Wraps an ADO.NET transaction to implements the <c>ITransaction</c> interface
	/// </summary>
	/// <remarks>
	/// TODO: DESIGNQUESTION: We might want to rename this AdoTransaction to indicate this Transaction object is specific
	/// to ADO.NET.  It seems like in the Java world your Transaction objects are much more flexible - allowing
	/// for JDBC Transactions or JTA Transactions.  
	/// </remarks>
	public class Transaction : ITransaction 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Transaction));
		private ISessionImplementor session;
		private IDbTransaction trans;
		private bool begun;
		private bool committed;
		private bool rolledBack;

		public Transaction(ISessionImplementor session) 
		{
			this.session = session;
		}

		public void Enlist(IDbCommand command) 
		{
			if( trans==null ) 
			{
				if( log.IsWarnEnabled ) 
				{
					if( command.Transaction!=null ) 
					{
						log.Warn("set a nonnull IDbCommand.Transaction to null because the Session had no Transaction");
					}
				}

				command.Transaction = null;
				return;
			}
			else 
			{
				if( log.IsWarnEnabled ) 
				{
					// got into here because the command was being initialized and had a null Transaction - probably
					// don't need to be confused by that - just a normal part of initialization...
					if( command.Transaction!=null && command.Transaction!=trans ) 
					{
						log.Warn("The IDbCommand had a different Transaction than the Session.  This can occur when " +
							"Disconnecting and Reconnecting Sessions because the PreparedCommand Cache is Session specific.");
					}
				}
		
				command.Transaction = trans; 
			}
		}

		public void Begin() 
		{
			log.Debug("begin");

			try 
			{
				IsolationLevel isolation = session.Factory.Isolation;
				if( isolation==IsolationLevel.Unspecified ) 
				{
					trans = session.Connection.BeginTransaction();
				}
				else 
				{
					trans = session.Connection.BeginTransaction( isolation );
				}
			} 
			catch( Exception e ) 
			{
				log.Error("Begin transaction failed", e);
				throw new TransactionException("Begin failed with SQL exception", e);
			}

			begun = true;
		}

		public void Commit() 
		{
			if (!begun) 
			{
				throw new TransactionException("Transaction not successfully started");
			}

			log.Debug("commit");

			try 
			{
				if( session.FlushMode!=FlushMode.Never ) 
				{
					session.Flush();
				}
				try 
				{
					trans.Commit();
					committed = true;
				} 
				catch( Exception e ) 
				{
					log.Error("Commit failed", e);
					throw new TransactionException("Commit failed with SQL exception", e);
				}
			} 
			finally 
			{
				session.AfterTransactionCompletion();
			}
		}

		public void Rollback() 
		{
			if (!begun) 
			{
				throw new TransactionException("Transaction not successfully started");
			}

			log.Debug("rollback");
			try 
			{
				trans.Rollback();
				rolledBack = true;
			} 
			catch( Exception e ) 
			{
				log.Error("Rollback failed", e);
				throw new TransactionException("Rollback failed with SQL Exception", e);
			} 
			finally 
			{
				session.AfterTransactionCompletion();
			}
		}

		public bool WasRolledBack 
		{
			get { return rolledBack; }
		}

		public bool WasCommitted 
		{
			get { return committed; }
		}

	}
}
