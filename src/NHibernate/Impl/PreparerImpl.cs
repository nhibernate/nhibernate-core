using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Impl 
{
	/// <summary>
	/// The implementing class for the Interface IPreparer.
	/// </summary>
	/// <remarks>
	/// This provides a Session level cache of SqlString/string containing sql to an IDbCommand that has been built with
	/// it.
	/// </remarks>
	internal class PreparerImpl: IPreparer 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PreparerImpl));
		private readonly ISessionFactoryImplementor factory;
		private readonly ISessionImplementor session;
		
		// key = SqlString or a sql string
		// value = ADO.NET Command

		// A prepared command is ADO.NET command that is attached to an IDbConnection
		// and optionally an IDbTransaction
		private Hashtable preparedCommands = new Hashtable();

		private IDbConnection currentConnection;

		public PreparerImpl(ISessionFactoryImplementor factory, ISessionImplementor session)
		{
			this.factory = factory;
			this.session = session;
		}

		/// <summary>
		/// The IDbConnection to use when building an preparing the Commands.
		/// </summary>
		/// <remarks>If the Connection ever changes then we need to clear out the hashtables.</remarks>
		private IDbConnection DbConnection 
		{
			get 
			{
				return this.currentConnection;
			}
			set 
			{
				if(currentConnection!=value) 
				{
					if(currentConnection!=null) 
					{
						log.Warn("The current connection was not the same at the Connection from the Session");
					}

					// reset the prepared Commands because they are specific to
					// a connection.
					preparedCommands = new Hashtable();

					currentConnection = value;
				}
				else 
				{
					log.Debug("PrepareImpl is using the same connection as the Session");
				}
			}
		}

		private IDbCommand BuildCommand(string sql) 
		{
			IDbCommand cmd = factory.ConnectionProvider.Driver.GenerateCommand(factory.Dialect, sql);
			if(log.IsDebugEnabled) 
			{
				log.Debug( "Building an IDbCommand object for the sql: " + sql );
			}
			return cmd;
			
		}


		private IDbCommand BuildCommand(SqlString sqlString) 
		{
			IDbCommand cmd = factory.ConnectionProvider.Driver.GenerateCommand(factory.Dialect, sqlString);
			if(log.IsDebugEnabled) 
			{
				log.Debug( "Building an IDbCommand object for the SqlString: " + sqlString.ToString() );
			}
			return cmd;
			
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

			return command;
		}

		public IDbCommand PrepareCommand(IDbCommand dbCommand)
		{
			try 
			{
				if(log.IsInfoEnabled) 
				{
					log.Info(dbCommand.CommandText);
				}

				dbCommand.Connection = this.DbConnection;
				dbCommand = JoinTransaction(dbCommand);
			
				if(factory.ConnectionProvider.Driver.SupportsPreparingCommands) 
				{
					dbCommand.Prepare();
				}

				return dbCommand;
			}
			catch(Exception e) 
			{
				throw new ApplicationException(
					"While preparing " + dbCommand.CommandText + " an error occurred"
					, e);
			}
		}

		public IDbCommand PrepareCommand(string sql) 
		{
			this.DbConnection = session.Connection;
			IDbCommand cmd = preparedCommands[sql] as IDbCommand;

			if( cmd==null ) 
			{
				cmd = this.BuildCommand(sql);
			}

			cmd = PrepareCommand(cmd);
			preparedCommands[sql] = cmd;
			return cmd;
			
		}

		public IDbCommand PrepareCommand(SqlString sqlString) 
		{
			this.DbConnection = session.Connection;
			IDbCommand cmd = preparedCommands[sqlString] as IDbCommand;

			if( cmd==null ) 
			{
				cmd = this.BuildCommand(sqlString);
			}

			cmd = PrepareCommand(cmd);
			preparedCommands[sqlString] = cmd;
			return cmd;
				
		}

	}
}
