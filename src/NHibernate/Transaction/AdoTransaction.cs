using System;
using System.Data;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// Wraps an ADO.NET <see cref="IDbTransaction"/> to implement
	/// the <see cref="ITransaction" /> interface.
	/// </summary>
	public class AdoTransaction : ITransaction
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( AdoTransaction ) );
		private ISessionImplementor session;
		private IDbTransaction trans;
		private bool begun;
		private bool committed;
		private bool rolledBack;

		/// <summary>
		/// Initializes a new instance of the <see cref="AdoTransaction"/> class.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> the Transaction is for.</param>
		public AdoTransaction( ISessionImplementor session )
		{
			this.session = session;
		}

		/// <summary>
		/// Enlist the <see cref="IDbCommand"/> in the current <see cref="ITransaction"/>.
		/// </summary>
		/// <param name="command">The <see cref="IDbCommand"/> to enlist in this Transaction.</param>
		/// <remarks>
		/// This takes care of making sure the <see cref="IDbCommand"/>'s Transaction property 
		/// contains the correct <see cref="IDbTransaction"/> or <c>null</c> if there is no
		/// Transaction for the ISession - ie <c>BeginTransaction()</c> not called.
		/// </remarks>
		public void Enlist( IDbCommand command )
		{
			if( trans == null )
			{
				if( log.IsWarnEnabled )
				{
					if( command.Transaction != null )
					{
						log.Warn( "set a nonnull IDbCommand.Transaction to null because the Session had no Transaction" );
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
					if( command.Transaction != null && command.Transaction != trans )
					{
						log.Warn( "The IDbCommand had a different Transaction than the Session.  This can occur when " +
							"Disconnecting and Reconnecting Sessions because the PreparedCommand Cache is Session specific." );
					}
				}

				command.Transaction = trans;
			}
		}

		/// <summary>
		/// Begins the <see cref="IDbTransaction"/> on the <see cref="IDbConnection"/>
		/// used by the <see cref="ISession"/>.
		/// </summary>
		/// <exception cref="TransactionException">
		/// Thrown if there is any problems encountered while trying to create
		/// the <see cref="IDbTransaction"/>.
		/// </exception>
		public void Begin( IsolationLevel isolationLevel )
		{
			log.Debug( "begin" );

			try
			{
				if( isolationLevel == IsolationLevel.Unspecified )
				{
					isolationLevel = session.Factory.Isolation;
				}

				if( isolationLevel == IsolationLevel.Unspecified )
				{
					trans = session.Connection.BeginTransaction();
				}
				else
				{
					trans = session.Connection.BeginTransaction( isolationLevel );
				}
			}
			catch( Exception e )
			{
				log.Error( "Begin transaction failed", e );
				throw new TransactionException( "Begin failed with SQL exception", e );
			}

			begun = true;
		}

		/// <summary>
		/// Commits the <see cref="ITransaction"/> by flushing the <see cref="ISession"/>
		/// and committing the <see cref="IDbTransaction"/>.
		/// </summary>
		/// <exception cref="TransactionException">
		/// Thrown if there is any exception while trying to call <c>Commit()</c> on 
		/// the underlying <see cref="IDbTransaction"/>.
		/// </exception>
		public void Commit()
		{
			if( !begun )
			{
				throw new TransactionException( "Transaction not successfully started" );
			}

			if( rolledBack || committed )
			{
				throw new TransactionException( "Transaction already finished" );
			}

			log.Debug( "commit" );

			try
			{
				if( session.FlushMode != FlushMode.Never )
				{
					session.Flush();
				}
				try
				{
					trans.Commit();
					committed = true;
					Dispose();
				}
				catch( Exception e )
				{
					log.Error( "Commit failed", e );
					throw new TransactionException( "Commit failed with SQL exception", e );
				}
			}
			finally
			{
				session.AfterTransactionCompletion( true );
			}
		}

		/// <summary>
		/// Rolls back the <see cref="ITransaction"/> by calling the method <c>Rollback</c> 
		/// on the underlying <see cref="IDbTransaction"/>.
		/// </summary>
		/// <exception cref="TransactionException">
		/// Thrown if there is any exception while trying to call <c>Rollback()</c> on 
		/// the underlying <see cref="IDbTransaction"/>.
		/// </exception>
		public void Rollback()
		{
			if( !begun )
			{
				throw new TransactionException( "Transaction not successfully started" );
			}

			if( rolledBack || committed )
			{
				throw new TransactionException( "Transaction already finished" );
			}

			log.Debug( "rollback" );
			try
			{
				trans.Rollback();
				rolledBack = true;
				Dispose();
			}
			catch( Exception e )
			{
				log.Error( "Rollback failed", e );
				throw new TransactionException( "Rollback failed with SQL Exception", e );
			}
			finally
			{
				session.AfterTransactionCompletion( false );
			}
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the transaction was rolled back.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="IDbTransaction"/> had <c>Rollback</c> called
		/// without any exceptions.
		/// </value>
		public bool WasRolledBack
		{
			get { return rolledBack; }
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the transaction was committed.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="IDbTransaction"/> had <c>Commit</c> called
		/// without any exceptions.
		/// </value>
		public bool WasCommitted
		{
			get { return committed; }
		}

		public IsolationLevel IsolationLevel
		{
			get { return trans.IsolationLevel; }
		}

		#region System.IDisposable Members
		
		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~AdoTransaction()
		{
			Dispose( false );
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			log.Debug( "running AdoTransaction.Dispose()" );
			Dispose( true );
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this AdoTransaction is being Disposed of or Finalized.</param>
		/// <remarks>
		/// If this AdoTransaction is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this AdoTransaction back to life.
		/// </remarks>
		protected virtual void Dispose(bool isDisposing)
		{
			if( _isAlreadyDisposed )
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the AdoTransaction if we
			// know this call came through Dispose()
			if( isDisposing )
			{
				if( trans!=null ) 
				{
					trans.Dispose();
				}
			}

			// free unmanaged resources here
			
			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize( this );
			
		}
		#endregion

	}
}