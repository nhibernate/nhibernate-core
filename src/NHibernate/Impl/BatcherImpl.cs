using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Impl {

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
	internal abstract class BatcherImpl : IBatcher {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BatcherImpl));

		public static int open;

		protected ISessionImplementor session;
		protected ISessionFactoryImplementor factory;

		// batchCommand used to be called batchUpdate - that name to me implied that updates
		// were being sent - however this could be just INSERT/DELETE/SELECT SQL statement not
		// just update.  However I haven't seen this being used with read statements...
		private IDbCommand batchCommand;
		private string batchCommandSQL;

		private ArrayList statementsToClose = new ArrayList();

		public BatcherImpl(ISessionImplementor session) {
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
		protected IDbCommand GetStatement() {
			return batchCommand;
		}

		public IDbCommand PrepareStatement(string sql) {
			
			ExecuteBatch();
			LogOpen();
			
			return JoinTransaction( factory.GetPreparedStatement( session.Connection, sql, false) );

		}
		public IDbCommand PrepareQueryStatement(string sql) {
			LogOpen();
			IDbCommand command = factory.GetPreparedStatement( session.Connection, sql, false );
			
			factory.SetFetchSize(command);
			statementsToClose.Add(command);
			
			return JoinTransaction(command);
		}

		/// <summary>
		/// Joins the Command to the Transaction and ensures that the Session and IDbCommand are in 
		/// the same Transaction.
		/// </summary>
		/// <param name="command">The command to setup the Transaction on.</param>
		/// <returns>A IDbCommand with a valid Transaction property.</returns>
		private IDbCommand JoinTransaction(IDbCommand command)
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
				
				if(command.Transaction!=null) log.Warn("set a nonnull IDbCommand.Transaction to null because the Session had no Transaction");
				command.Transaction = null;

			}

			// make sure these are the same transaction - I don't know why we would have a command
			// in a different Transaction than the Session, but I don't understand all of the code
			// well enough yet to verify that.
			else if (sessionAdoTrx!=command.Transaction) 
			{
				// got into here because the command was being initialized and had a null Transaction - probably
				// don't need to be confused by that - just a normal part of initialization...
				log.Warn("The IDbCommand had a different Transaction than the Session.  What is going on???");
				command.Transaction = sessionAdoTrx; 
			}

			return command;
		}

		public void CloseQueryStatement(IDbCommand st) {
			statementsToClose.Remove(st);
			LogClose();
			factory.ClosePreparedStatement(st);
		}

		public void CloseStatement(IDbCommand ps) {
			LogClose();
			factory.ClosePreparedStatement(ps);
		}

		public IDbCommand PrepareBatchStatement(string sql) {
			if ( !sql.Equals(batchCommandSQL) ) {
				batchCommand = PrepareStatement(sql);
				batchCommandSQL=sql;
			}
			return batchCommand;
		}

		public void ExecuteBatch() {
			if ( batchCommand!=null ) {
				IDbCommand ps = batchCommand;
				batchCommand = null;
				batchCommandSQL = null;
				try {
					DoExecuteBatch(ps);
				} finally {
					CloseStatement(ps);
				}
			}
		}

		public void CloseStatements() {
			foreach( IDbCommand cmd in statementsToClose ) {
				try {
					CloseStatement(cmd);
				} catch(Exception e) {
					// no big deal
					log.Warn("Could not close a JDBC statement", e);
				}
			}
			statementsToClose.Clear();
		}

		protected abstract void DoExecuteBatch(IDbCommand ps) ;
		public abstract void AddToBatch(int expectedCount);

		private static void LogOpen() {
			if ( log.IsDebugEnabled ) {
				open++;
				log.Debug( open + " open PreparedStatements" );
			}
		}

		private static void LogClose() {
			if ( log.IsDebugEnabled )
				open--;
		}
	}
}
