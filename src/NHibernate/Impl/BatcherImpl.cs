using System;
using System.Collections;
using System.Data;
using Iesi.Collections;
using log4net;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Impl
{
	/// <summary>
	/// Manages prepared statements and batching. Class exists to enfores seperation of concerns
	/// </summary>
	internal abstract class BatcherImpl : IBatcher
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( BatcherImpl ) );

		private static int openCommandCount;
		private static int openReaderCount;

		private readonly ISessionImplementor session;

		private readonly ISessionFactoryImplementor factory;

		// batchCommand used to be called batchUpdate - that name to me implied that updates
		// were being sent - however this could be just INSERT/DELETE/SELECT SQL statement not
		// just update.  However I haven't seen this being used with read statements...
		private IDbCommand batchCommand;
		private SqlString batchCommandSql;

		private ISet commandsToClose = new HashedSet();
		private ISet readersToClose = new HashedSet();

		/// <summary>
		/// An IDictionary with a key of a SqlString and 
		/// a value of an IDbCommand.
		/// </summary>
		private IDictionary commands;

		/// <summary>
		/// Initializes a new instance of the <see cref="BatcherImpl"/> class.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this Batcher is executing in.</param>
		public BatcherImpl( ISessionImplementor session )
		{
			this.session = session;
			this.factory = session.Factory;
			commands = new Hashtable();
		}

		/// <summary>
		/// Gets the current <see cref="IDbCommand"/> that is contained for this Batch
		/// </summary>
		/// <value>The current <see cref="IDbCommand"/>.</value>
		protected IDbCommand CurrentCommand
		{
			// in h2.0.3 this was a method GetCommand
			get { return batchCommand; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlString"></param>
		/// <returns></returns>
		public IDbCommand Generate( SqlString sqlString )
		{
			IDbCommand cmd = commands[ sqlString ] as IDbCommand;

			if( cmd != null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Using prebuilt IDbCommand object for the SqlString: " + sqlString.ToString() );
				}

				return cmd;
			}

			// need to build the IDbCommand from the sqlString bec
			cmd = factory.ConnectionProvider.Driver.GenerateCommand( factory.Dialect, sqlString );
			if( log.IsDebugEnabled )
			{
				log.Debug( "Building an IDbCommand object for the SqlString: " + sqlString.ToString() );
			}

			commands[ sqlString ] = cmd;
			return cmd;

		}

		/// <summary>
		/// Prepares the <see cref="IDbCommand"/> for execution in the database.
		/// </summary>
		/// <param name="command"></param>
		/// <remarks>
		/// This takes care of hooking the <see cref="IDbCommand"/> up to an <see cref="IDbConnection"/>
		/// and <see cref="IDbTransaction"/> if one exists.  It will call <c>Prepare</c> if the Driver
		/// supports preparing commands.
		/// </remarks>
		private void Prepare( IDbCommand command )
		{
			try
			{
				if( log.IsInfoEnabled )
				{
					log.Info( "Preparing " + command.CommandText );
				}

				if( command.Connection != null )
				{
					// make sure the commands connection is the same as the Sessions connection
					// these can be different when the session is disconnected and then reconnected
					if( command.Connection != session.Connection )
					{
						command.Connection = session.Connection;
					}
				}
				else
				{
					command.Connection = session.Connection;
				}

				if( session.Transaction != null )
				{
					session.Transaction.Enlist( command );
				}

				if( factory.PrepareSql && factory.ConnectionProvider.Driver.SupportsPreparingCommands )
				{
					command.Prepare();
				}
			}
			catch( InvalidOperationException ioe )
			{
				throw new ADOException(
					"While preparing " + command.CommandText + " an error occurred"
					, ioe );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public IDbCommand PrepareBatchCommand( SqlString sql )
		{
			if( !sql.Equals( batchCommandSql ) )
			{
				batchCommand = PrepareCommand( sql ); // calls ExecuteBatch()
				batchCommandSql = sql;
			}
			return batchCommand;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public IDbCommand PrepareCommand( SqlString sql )
		{
			// a new IDbCommand is being prepared and a new (potential) batch
			// started - so execute the current batch of commands.
			ExecuteBatch();

			LogOpenPreparedCommand();

			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			return Generate( sql );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="scrollable"></param>
		/// <returns></returns>
		public IDbCommand PrepareQueryCommand( SqlString sql, bool scrollable )
		{
			//TODO: figure out what to do with scrollable - don't think it applies
			// to ado.net since DataReader is forward only
			LogOpenPreparedCommand();

			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			IDbCommand command = Generate( sql );

			commandsToClose.Add( command );

			return command;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public void AbortBatch( Exception e )
		{
			// log the exception here
			IDbCommand cmd = batchCommand;
			batchCommand = null;
			batchCommandSql = null;
			CloseCommand( cmd, null );
			// close the statement closeStatement(cmd)
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public int ExecuteNonQuery( IDbCommand cmd )
		{
			int rowsAffected = 0;

			CheckReaders();

			Prepare( cmd );
			rowsAffected = cmd.ExecuteNonQuery();

			return rowsAffected;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public IDataReader ExecuteReader( IDbCommand cmd )
		{
			CheckReaders();

			Prepare( cmd );
			

			IDataReader reader;
			if( factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders == false )
			{
				reader = new NHybridDataReader( cmd.ExecuteReader() );
			}
			else
			{
				reader = cmd.ExecuteReader();
			}

			readersToClose.Add( reader );
			LogOpenReader();
			return reader;
		}

		/// <summary>
		/// Ensures that the Driver's rules for Multiple Open DataReaders are being followed.
		/// </summary>
		private void CheckReaders()
		{
			// early exit because we don't need to move an open IDataReader into memory
			// since the Driver supports mult open readers.
			if( factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders )
			{
				return;
			}

			foreach( NHybridDataReader reader in readersToClose )
			{
				reader.ReadIntoMemory();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="reader"></param>
		public void CloseCommand( IDbCommand cmd, IDataReader reader )
		{
			//TODO: fix this up a little bit - don't like it having the same name and just
			// turning around and calling a diff method.
			CloseQueryCommand( cmd, reader );
			// CloseQueryCommand contains the logging so we don't need to call it 
			// here - putting it in CloseQueryCommand(IDbCommand) will ensure it always gets called.
			//LogClosePreparedCommands(); 
		}

		/// <summary>
		/// 
		/// </summary>
		public void CloseCommands()
		{
			foreach( IDataReader reader in readersToClose )
			{
				try
				{
					LogCloseReader();
					reader.Close();
				}
				catch( Exception e )
				{
					log.Warn( "Could not close IDataReader", e );
				}
			}
			readersToClose.Clear();

			foreach( IDbCommand cmd in commandsToClose )
			{
				try
				{
					CloseQueryCommand( cmd );
				}
				catch( Exception e )
				{
					// no big deal
					log.Warn( "Could not close ADO.NET Command", e );
				}
			}
			commandsToClose.Clear();
		}

		private void CloseQueryCommand( IDbCommand cmd )
		{
			try
			{
				// no equiv to the java code in here
			}
			catch( Exception e )
			{
				log.Warn( "exception clearing maxRows/queryTimeout", e );
				//cmd.close();  if there was a close method in command
				return; // NOTE: early exit!
			}

			LogClosePreparedCommand();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="reader"></param>
		public void CloseQueryCommand( IDbCommand st, IDataReader reader )
		{
			commandsToClose.Remove( st );
			if( reader != null )
			{
				readersToClose.Remove( reader );
			}

			try
			{
				if( reader != null )
				{
					LogCloseReader();
					reader.Close();
				}
			}
			finally
			{
				CloseQueryCommand( st );
			}
		}

		/// <summary></summary>
		public void ExecuteBatch()
		{
			// if there is currently a command that a batch is
			// being built for then execute it
			if( batchCommand != null )
			{
				IDbCommand ps = batchCommand;
				batchCommand = null;
				batchCommandSql = null;
				try
				{
					DoExecuteBatch( ps );
				}
				finally
				{
					CloseCommand( ps, null );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ps"></param>
		protected abstract void DoExecuteBatch( IDbCommand ps );

		/// <summary>
		/// Adds the expected row count into the batch.
		/// </summary>
		/// <param name="expectedRowCount">The number of rows expected to be affected by the query.</param>
		/// <remarks>
		/// If Batching is not supported, then this is when the Command should be executed.  If Batching
		/// is supported then it should hold of on executing the batch until explicitly told to.
		/// </remarks>
		public abstract void AddToBatch( int expectedRowCount );

		/// <summary>
		/// Gets the <see cref="ISessionFactoryImplementor"/> the Batcher was
		/// created in.
		/// </summary>
		/// <value>
		/// The <see cref="ISessionFactoryImplementor"/> the Batcher was
		/// created in.
		/// </value>
		protected ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		/// <summary>
		/// Gets the <see cref="ISessionImplementor"/> the Batcher is handling the 
		/// sql actions for.
		/// </summary>
		/// <value>
		/// The <see cref="ISessionImplementor"/> the Batcher is handling the 
		/// sql actions for.
		/// </value>
		protected ISessionImplementor Session
		{
			get { return session; }
		}

		private static void LogOpenPreparedCommand()
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "about to open: " + openCommandCount + " open IDbCommands, " + openReaderCount + " open DataReaders" );
				openCommandCount++;
			}
		}

		private static void LogClosePreparedCommand()
		{
			if( log.IsDebugEnabled )
			{
				openCommandCount--;
				log.Debug( "done closing: " + openCommandCount + " open IDbCommands, " + openReaderCount + " open DataReaders" );
			}
		}

		private static void LogOpenReader()
		{
			if( log.IsDebugEnabled )
			{
				openReaderCount++;
			}
		}

		private static void LogCloseReader()
		{
			if( log.IsDebugEnabled )
			{
				openReaderCount--;
			}
		}

		#region IDisposable Members
		
		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~BatcherImpl()
		{
			Dispose( false );
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			log.Debug( "running BatcherImpl.Dispose()" );
			Dispose( true );
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this BatcherImpl is being Disposed of or Finalized.</param>
		/// <remarks>
		/// If this BatcherImpl is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this BatcherImpl back to life.
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
				foreach( IDataReader reader in readersToClose )
				{
					try
					{
						LogCloseReader();
						reader.Dispose();
					}
					catch( Exception e )
					{
						log.Warn( "Could not dispose IDataReader", e );
					}
				}
				readersToClose.Clear();

				foreach( IDbCommand cmd in commandsToClose )
				{
					try
					{
						LogClosePreparedCommand();
						cmd.Dispose();
					}
					catch( Exception e )
					{
						// no big deal
						log.Warn( "Could not dispose of ADO.NET Command", e );
					}
				}
				commandsToClose.Clear();
			}

			// free unmanaged resources here
			
			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize( this );
			
		}

		#endregion


	}
}