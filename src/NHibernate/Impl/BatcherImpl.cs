using System;
using System.Collections;
using System.Data;

using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Impl 
{
	/// <summary>
	/// Manages prepared statements and batching. Class exists to enfores seperation of concerns
	/// TODO: RESEARCH how ADO.NET batching compares to JDBC batching - I am not at all familiar with
	/// this concept nor where/how it is used in Hibernate
	/// 
	/// From reading the Hibernate source code it looks like Java's JDBC drivers have the ability
	/// to process SQL Statements in batches.  I have looked through the newsgroups and documentation
	/// and it doesn't appear that ADO.Net has any similar concept.  It might be convenient just to leave
	/// this in here and use the NonBatchingBatcher.
	/// 
	/// http://java.sun.com/docs/books/tutorial/jdbc/jdbc2dot0/batchupdates.html explains how PreparedStatements
	/// handle batch updating - I don't see that concept in ADO.NET at all because the main interface is the IDbCommand
	/// and it handles a command - about the only way to batch would be just to keep adding on to the CommandText and
	/// adding more Parameters.  I think that would get a little ugly for the performance gain - don't know what the gain
	/// would be because I don't want to even think about writing that code :)
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

		private ArrayList commandsToClose = new ArrayList();
		private ArrayList readersToClose = new ArrayList();

		public BatcherImpl(ISessionImplementor session) 
		{
			this.session = session;
			this.factory = session.Factory;
		}

		/// <summary>
		/// Gets the current Command that is contained for this Batch
		/// </summary>
		/// <remarks>
		/// In the java version you make a PreparedStatement and then append values to the 
		/// parameters such as:
		/// ps.setInt(1, 1);
		/// ps.setString(2, "Second Param");
		/// ps.addBatch();
		/// If I am reading the javadoc correctly now you can go back to the PreparedStatement
		/// and add more values such as
		/// ps.setInt(1, 2);
		/// ps.SetString(2, "Second Param, on Second evaluation of PreparedStatement);
		/// ps.addBatch();
		/// 
		/// </remarks>
		/// <returns></returns>
		protected IDbCommand GetCommand() 
		{
			return batchCommand;
		}
		
		public IDbCommand Generate(SqlString sqlString) 
		{
			IDbCommand cmd = factory.ConnectionProvider.Driver.GenerateCommand(factory.Dialect, sqlString);
			if(log.IsDebugEnabled) 
			{
				log.Debug( "Building an IDbCommand object for the SqlString: " + sqlString.ToString() );
			}
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

				command.Connection = session.Connection;
				JoinTransaction( command );
			
				if( factory.ConnectionProvider.Driver.SupportsPreparingCommands ) 
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

		/// <summary>
		/// Joins the Command to the Transaction and ensures that the Session and IDbCommand are in 
		/// the same Transaction.
		/// </summary>
		/// <param name="command">The command to setup the Transaction on.</param>
		/// <returns>A IDbCommand with a valid Transaction property.</returns>
		/// TODO: move this into ITransaction and let the Transaction figure out how to
		/// get the Command to be a part of it.  When .net 2.0 and the new transaction interface
		/// comes out it will probably have its own strategy...
		private void JoinTransaction(IDbCommand command) 
		{
			IDbTransaction sessionAdoTrx = null;
			
			// at this point in the code if the Transaction is not null then we know we
			// have a Transaction object that has the .AdoTransaction property.  In the future
			// we will have a seperate object to represent an AdoTransaction and won't have a 
			// generic Transaction class - the existing Transaction class will become Abstract.
			if(this.session.Transaction!=null) sessionAdoTrx = ((Transaction.Transaction)session.Transaction).AdoTransaction;


			// if the sessionAdoTrx is null then we don't want the command to be a part of
			// any Transaction - so lets set the command trx to null
			if(sessionAdoTrx==null) 
			{
				if(command.Transaction!=null) 
				{
					log.Warn("set a nonnull IDbCommand.Transaction to null because the Session had no Transaction");
				}
				command.Transaction = null;
			}

			// make sure these are the same transaction - I don't know why we would have a command
			// in a different Transaction than the Session, but I don't understand all of the code
			// well enough yet to verify that.
			else if (sessionAdoTrx!=command.Transaction) 
			{
				
				// got into here because the command was being initialized and had a null Transaction - probably
				// don't need to be confused by that - just a normal part of initialization...
				if(command.Transaction!=null) 
				{
					log.Warn("The IDbCommand had a different Transaction than the Session.  This can occur when " +
						"Disconnecting and Reconnecting Sessions because the PreparedCommand Cache is Session specific.");
				}
		
				command.Transaction = sessionAdoTrx; 
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
			IDbCommand command = Generate( sql ); // session.Preparer.BuildCommand(sql);
			
			// not sure if this is needed because fetch size doesn't apply
			factory.SetFetchSize(command);
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

			for( int i=0; i<readersToClose.Count; i++ ) 
			{
				((NHybridDataReader)readersToClose[i]).ReadIntoMemory();
			}
			
		}

		public void CloseCommand(IDbCommand cmd, IDataReader reader) 
		{
			//TODO: fix this up a little bit - don't like it having the same name and just
			// turning around and calling a diff method.
			CloseQueryCommand( cmd, reader );
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

//			CloseCommand( cmd, null );
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
			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "about to open: " + openCommandCount + " open IDbCommands, " + openReaderCount + " open DataReaders" );
			}
			openCommandCount++;
		}

		private static void LogClosePreparedCommands() 
		{
			openCommandCount--;

			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "done closing: " + openCommandCount + " open IDbCommands, " + openReaderCount + " open DataReaders" );
			}
		}

		private static void LogOpenReaders() 
		{
			openReaderCount++;
		}

		private static void LogCloseReaders() 
		{
			openReaderCount--;
		}

		
	}
}
