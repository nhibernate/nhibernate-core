using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Impl {
	
	/// <summary>
	/// The implementing class for the Interface IPreparer.
	/// </summary>
	internal class PreparerImpl: IPreparer {
		
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PreparerImpl));
		
		private ISessionFactoryImplementor factory;
		private ISessionImplementor session;

		// key = SqlString or a sql string
		// value = ADO.NET Command
		// A Built command is just an sql string/SQL Statement that has been converted
		// into an ADO.NET Command
		private Hashtable builtCommands = new Hashtable();

		// A prepared command is ADO.NET command that is attached to an IDbConnection
		// and optionally an IDbTransaction
		private Hashtable preparedCommands = new Hashtable();

		private IDbConnection currentConnection;

		public PreparerImpl(ISessionFactoryImplementor factory, ISessionImplementor session){
			this.factory = factory;
			this.session = session;
			
		}

		/// <summary>
		/// The IDbConnection to use when building an preparing the Commands.
		/// </summary>
		/// <remarks>If the Connection ever changes then we need to clear out the hashtables.</remarks>
		private IDbConnection DbConnection {
			get {
				return this.currentConnection;
			}
			set {
				if(currentConnection!=value) {
					
					if(currentConnection!=null)
						log.Warn("The current connection was not the same at the Connection from the Session");

					// reset the prepared Commands because they are specific to
					// a connection.
					preparedCommands = new Hashtable();
					builtCommands = new Hashtable();

					currentConnection = value;
				}
				else {
					log.Info("PrepareImpl is using the same connection as the Session");
				}
			}
		}

		public IDbCommand BuildCommand(string sql) {
			if(builtCommands.ContainsKey(sql)) {
				return (IDbCommand)builtCommands[sql];
			}
			else {
				IDbCommand cmd = factory.ConnectionProvider.Driver.CreateCommand();
				cmd.CommandText = sql;
				
				builtCommands.Add(sql, cmd);
				return cmd;
			}

		}


		public IDbCommand BuildCommand(SqlString sqlString) 
		{
			int paramIndex = 0;

			if(builtCommands.ContainsKey(sqlString)) 
				return (IDbCommand) builtCommands[sqlString];
			
			IDbCommand cmd = factory.ConnectionProvider.Driver.CreateCommand();

			StringBuilder builder = new StringBuilder(sqlString.SqlParts.Length * 15);
			for(int i = 0; i < sqlString.SqlParts.Length; i++) 
			{
				object part = sqlString.SqlParts[i];
				Parameter parameter = part as Parameter;
				
				if(parameter!=null) 
				{
					string paramName = "p" + paramIndex;
					builder.Append( parameter.GetSqlName(factory.ConnectionProvider, paramName) );
					IDbDataParameter dbParam = parameter.GetIDbDataParameter(cmd, factory.ConnectionProvider, paramName);
					cmd.Parameters.Add(dbParam);
					
					paramIndex++;
				}
				else 
				{
					builder.Append((string)part);
				}
			}

			cmd.CommandText = builder.ToString();

			builtCommands.Add(sqlString, cmd);
			return cmd;
			
		}

		/// <summary>
		/// Joins the Command to the Transaction and ensures that the Session and IDbCommand are in 
		/// the same Transaction.
		/// </summary>
		/// <param name="command">The command to setup the Transaction on.</param>
		/// <returns>A IDbCommand with a valid Transaction property.</returns>
		private IDbCommand JoinTransaction(IDbCommand command) {

			IDbTransaction sessionAdoTrx = null;
			
			// at this point in the code if the Transaction is not null then we know we
			// have a Transaction object that has the .AdoTransaction property.  In the future
			// we will have a seperate object to represent an AdoTransaction and won't have a 
			// generic Transaction class - the existing Transaction class will become Abstract.
			if(this.session.Transaction!=null) sessionAdoTrx = ((Transaction.Transaction)session.Transaction).AdoTransaction;


			// if the sessionAdoTrx is null then we don't want the command to be a part of
			// any Transaction - so lets set the command trx to null
			if(sessionAdoTrx==null) {
				
				if(command.Transaction!=null) log.Warn("set a nonnull IDbCommand.Transaction to null because the Session had no Transaction");
				command.Transaction = null;

			}

			// make sure these are the same transaction - I don't know why we would have a command
			// in a different Transaction than the Session, but I don't understand all of the code
			// well enough yet to verify that.
			else if (sessionAdoTrx!=command.Transaction) {
				
				// got into here because the command was being initialized and had a null Transaction - probably
				// don't need to be confused by that - just a normal part of initialization...
				if(command.Transaction!=null) 
					log.Warn("The IDbCommand had a different Transaction than the Session.");
		
				command.Transaction = sessionAdoTrx; 
			}

			return command;
		}

		public IDbCommand PrepareCommand(IDbCommand dbCommand){

			try {
				dbCommand.Connection = this.DbConnection;
				dbCommand = JoinTransaction(dbCommand);
			
				// TODO: remove this comment for Prepare() once we have fixed up the SqlTypes
				// and are able to differentiate between a Command that can be prepared and 
				// one that cannot.
				// for example - with SqlServer2000 a Command with a binary type 
				dbCommand.Prepare();

				return dbCommand;
			}
			catch(Exception e) {
				throw new ApplicationException(
					"While preparing " + dbCommand.CommandText + " an error occurred"
					, e);
			}
		}


		public IDbCommand PrepareCommand(string sql) {
			this.DbConnection = session.Connection;
			IDbCommand cmd = null;

			if(preparedCommands.ContainsKey(sql)) 
			{
				return (IDbCommand)preparedCommands[sql];
			}
			
			cmd = this.BuildCommand(sql);
			cmd = PrepareCommand(cmd);
			preparedCommands.Add(sql, cmd);
			return cmd;
			

		}

		public IDbCommand PrepareCommand(SqlString sqlString) {
			this.DbConnection = session.Connection;
			IDbCommand cmd = null;

			if(preparedCommands.ContainsKey(sqlString)) 
			{
				 return (IDbCommand)preparedCommands[sqlString];
			}

			cmd = this.BuildCommand(sqlString);
			cmd = PrepareCommand(cmd);
			preparedCommands.Add(sqlString, cmd);

			return cmd;
				
		}

	}
}
