using System;
using System.Collections;
using System.Data;

using Iesi.Collections;

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
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BatcherImpl));

		private static int openCommandCount;
		private static int openReaderCount;

		protected readonly ISessionImplementor session;
		protected readonly ISessionFactoryImplementor factory;

		// batchCommand used to be called batchUpdate - that name to me implied that updates
		// were being sent - however this could be just INSERT/DELETE/SELECT SQL statement not
		// just update.  However I haven't seen this being used with read statements...
		private IDbCommand batchCommand;
		private SqlString batchCommandSql;

		private ISet commandsToClose = new HashedSet();
		private ISet readersToClose = new HashedSet();

		// key = SqlString
		// value = IDbCommand
		private IDictionary commands;

		public BatcherImpl(ISessionImplementor session) 
		{
			this.session = session;
			this.factory = session.Factory;
			commands = new Hashtable();
		}

		/// <summary>
		/// Gets the current Command that is contained for this Batch
		/// </summary>
		protected IDbCommand GetCommand() 
		{
			return batchCommand;
		}
		
		public IDbCommand Generate(SqlString sqlString) 
		{
			IDbCommand cmd = commands[ sqlString ] as IDbCommand;

			if( cmd!=null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "Using prebuilt IDbCommand object for the SqlString: " + sqlString.ToString() );
				}

				return cmd;
			}

			// need to build the IDbCommand from the sqlString bec
			cmd = factory.ConnectionProvider.Driver.GenerateCommand(factory.Dialect, sqlString);
			if(log.IsDebugEnabled) 
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
		private void Prepare(IDbCommand command) 
		{
			try 
			{
				if(log.IsInfoEnabled) 
				{
					log.Info("Preparing " + command.CommandText);
				}

				if( command.Connection!=null ) 
				{
					// make sure the commands connection is the same as the Sessions connection
					// these can be different when the session is disconnected and then reconnected
					if( command.Connection!=session.Connection ) 
					{
						command.Connection = session.Connection;
					}
				}
				else 
				{
					command.Connection = session.Connection;
				}

				if( session.Transaction!=null ) 
				{
					session.Transaction.Enlist( command );
				}

				if( factory.PrepareSql && factory.ConnectionProvider.Driver.SupportsPreparingCommands ) 
				{
					command.Prepare();
				}
			}
			catch(Exception e) 
			{
				throw new ApplicationException(
					"While preparing " + command.CommandText + " an error occurred"
					, e);
			}
		}

		public IDbCommand PrepareBatchCommand(SqlString sql) 
		{
			if ( !sql.Equals(batchCommandSql) ) 
			{
				batchCommand = PrepareCommand(sql); // calls ExecuteBatch()
				batchCommandSql=sql;
			}
			return batchCommand;
		}

		public IDbCommand PrepareCommand(SqlString sql) 
		{
			ExecuteBatch();
			LogOpenPreparedCommands();
			
			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			return Generate( sql );
		}
		
		public IDbCommand PrepareQueryCommand(SqlString sql, bool scrollable) 
		{
			//TODO: figure out what to do with scrollable - don't think it applies
			// to ado.net since DataReader is forward only
			LogOpenPreparedCommands();

			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			IDbCommand command = Generate( sql ); 
			
			commandsToClose.Add(command);
			
			return command;
		}

		public void AbortBatch(Exception e) 
		{
			// log the exception here
			IDbCommand cmd = batchCommand;
			batchCommand = null;
			batchCommandSql = null;
			// close the statement closeStatement(cmd)
		}

		public int ExecuteNonQuery(IDbCommand cmd) 
		{
			int rowsAffected = 0;

			CheckReaders();

			Prepare( cmd );
			rowsAffected = cmd.ExecuteNonQuery();

			return rowsAffected;
		}

		public IDataReader ExecuteReader(IDbCommand cmd) 
		{
			CheckReaders();

			Prepare( cmd );;

			IDataReader reader;
			if( factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders==false ) 
			{
				reader = new NHybridDataReader( cmd.ExecuteReader() );
			}
			else 
			{
				reader = cmd.ExecuteReader();
			}

			readersToClose.Add(reader);
			LogOpenReaders();
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

		public void CloseCommand(IDbCommand cmd, IDataReader reader) 
		{
			//TODO: fix this up a little bit - don't like it having the same name and just
			// turning around and calling a diff method.
			CloseQueryCommand( cmd, reader );
			// CloseQueryCommand contains the logging so we don't need to call it 
			// here - putting it in CloseQueryCommand(IDbCommand) will ensure it always gets called.
			//LogClosePreparedCommands(); 
		}

		public void CloseCommands() 
		{
			foreach( IDataReader reader in readersToClose ) 
			{
				try 
				{
					LogCloseReaders();
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
					CloseQueryCommand(cmd);
				} 
				catch(Exception e) 
				{
					// no big deal
					log.Warn("Could not close a JDBC statement", e);
				}
			}
			commandsToClose.Clear();
		}

		private void CloseQueryCommand(IDbCommand cmd) 
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

			LogClosePreparedCommands();
		}

		public void CloseQueryCommand(IDbCommand st, IDataReader reader) 
		{
			commandsToClose.Remove(st);
			if( reader!=null ) 
			{
				readersToClose.Remove(reader);
			}

			try 
			{
				if( reader!=null) 
				{
					LogCloseReaders();
					reader.Close();
				}
			}
			finally 
			{
				CloseQueryCommand(st);
			}
		}

		public void ExecuteBatch() 
		{
			if ( batchCommand!=null ) 
			{
				IDbCommand ps = batchCommand;
				batchCommand = null;
				batchCommandSql = null;
				try 
				{
					DoExecuteBatch(ps);
				}
				finally 
				{
					CloseCommand(ps, null);
				}
			}
		}

		protected abstract void DoExecuteBatch(IDbCommand ps) ;
		public abstract void AddToBatch(int expectedRowCount);

		protected ISessionFactoryImplementor Factory 
		{
			get { return factory; }
		}

		protected ISessionImplementor Session 
		{
			get { return session; }
		}

		private static void LogOpenPreparedCommands() 
		{
			if( log.IsDebugEnabled ) 
			{
				log.Debug( "about to open: " + openCommandCount + " open IDbCommands, " + openReaderCount + " open DataReaders" );
				openCommandCount++;
			}
		}

		private static void LogClosePreparedCommands() 
		{
			if( log.IsDebugEnabled ) 
			{
				openCommandCount--;
				log.Debug( "done closing: " + openCommandCount + " open IDbCommands, " + openReaderCount + " open DataReaders" );
			}
		}

		private static void LogOpenReaders() 
		{
			if( log.IsDebugEnabled ) 
			{
				openReaderCount++;
			}
		}

		private static void LogCloseReaders() 
		{
			if( log.IsDebugEnabled ) 
			{
				openReaderCount--;
			}
		}

		
	}
}
