using System;
using System.Collections;
using System.Data;

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

		public IDbCommand PrepareCommand(SqlString sql) 
		{
			
			ExecuteBatch();
			LogOpenPreparedCommands();
			
			//return JoinTransaction( factory.GetPreparedStatement( session.Connection, sql, false) );
			return session.Preparer.PrepareCommand(sql);

		}
		
		public IDbCommand PrepareQueryCommand(SqlString sql, bool scrollable) 
		{
			//TODO: figure out what to do with scrollable - don't think it applies
			// to ado.net since DataReader is forward only
			LogOpenPreparedCommands();
			IDbCommand command = session.Preparer.PrepareCommand(sql);
			//factory.GetPreparedStatement( session.Connection, sql, false );
			
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

		public IDataReader GetDataReader(IDbCommand cmd) 
		{
			IDataReader reader = cmd.ExecuteReader();
			readersToClose.Add(reader);
			LogOpenReaders();
			return reader;
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

		public IDbCommand PrepareBatchCommand(SqlString sql) 
		{
			if ( !sql.Equals(batchCommandSql) ) 
			{
				batchCommand = PrepareCommand(sql); // calls ExecuteBatch()
				batchCommandSql=sql;
			}
			return batchCommand;
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
					CloseCommand(ps);
				}
			}
		}

		public void CloseCommand(IDbCommand cmd) 
		{
			LogClosePreparedCommands();
			// factory.ClosePreparedStatement(cmd);
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

			CloseCommand( cmd );
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
				openCommandCount++;
			}
		}

		private static void LogClosePreparedCommands() 
		{
			if ( log.IsDebugEnabled ) 
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
