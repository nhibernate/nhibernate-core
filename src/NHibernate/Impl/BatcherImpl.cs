using System;
using System.Data;
using Iesi.Collections;
using log4net;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using System.Text;
using NHibernate.SqlTypes;
namespace NHibernate.Impl
{
	/// <summary>
	/// Manages prepared statements and batching. Class exists to enforce separation of concerns
	/// </summary>
	internal abstract class BatcherImpl : IBatcher
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof(BatcherImpl));
		protected static readonly ILog logSql = LogManager.GetLogger("NHibernate.SQL");

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
		private IDbCommand lastQuery;

		/// <summary>
		/// Initializes a new instance of the <see cref="BatcherImpl"/> class.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this Batcher is executing in.</param>
		public BatcherImpl(ISessionImplementor session)
		{
			this.session = session;
			this.factory = session.Factory;
		}

		private IDriver Driver
		{
			get { return factory.ConnectionProvider.Driver; }
		}

		/// <summary>
		/// Gets the current <see cref="IDbCommand"/> that is contained for this Batch
		/// </summary>
		/// <value>The current <see cref="IDbCommand"/>.</value>
		protected IDbCommand CurrentCommand
		{
			get { return batchCommand; }
		}

		public IDbCommand Generate(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			IDbCommand cmd = factory.ConnectionProvider.Driver.GenerateCommand(type, sqlString, parameterTypes);
			LogOpenPreparedCommand();
			if (log.IsDebugEnabled)
			{
				log.Debug("Building an IDbCommand object for the SqlString: " + sqlString.ToString());
			}
			commandsToClose.Add(cmd);
			return cmd;
		}

		/// <summary>
		/// Prepares the <see cref="IDbCommand"/> for execution in the database.
		/// </summary>
		/// <remarks>
		/// This takes care of hooking the <see cref="IDbCommand"/> up to an <see cref="IDbConnection"/>
		/// and <see cref="IDbTransaction"/> if one exists.  It will call <c>Prepare</c> if the Driver
		/// supports preparing commands.
		/// </remarks>
		protected void Prepare(IDbCommand cmd)
		{
			try
			{
				Log(cmd);

				if (cmd.Connection != null)
				{
					// make sure the commands connection is the same as the Sessions connection
					// these can be different when the session is disconnected and then reconnected
					if (cmd.Connection != session.Connection)
					{
						cmd.Connection = session.Connection;
					}
				}
				else
				{
					cmd.Connection = session.Connection;
				}

				if (session.Transaction != null)
				{
					session.Transaction.Enlist(cmd);
				}

				Driver.PrepareCommand(cmd);
			}
			catch (InvalidOperationException ioe)
			{
				throw new ADOException(
					"While preparing " + cmd.CommandText + " an error occurred"
					, ioe);
			}
		}

		public IDbCommand PrepareBatchCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			if (!sql.Equals(batchCommandSql))
			{
				batchCommand = PrepareCommand(type, sql, parameterTypes); // calls ExecuteBatch()
				batchCommandSql = sql;
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("reusing command " + batchCommand.CommandText);
				}
			}

			return batchCommand;
		}

		public IDbCommand PrepareCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			// a new IDbCommand is being prepared and a new (potential) batch
			// started - so execute the current batch of commands.
			ExecuteBatch();

			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			return Generate(type, sql, parameterTypes);
		}

		public IDbCommand PrepareQueryCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			IDbCommand command = Generate(type, sql, parameterTypes);
			lastQuery = command;
			return command;
		}

		public void AbortBatch(Exception e)
		{
			IDbCommand cmd = batchCommand;
			batchCommand = null;
			batchCommandSql = null;
			// close the statement closeStatement(cmd)
			CloseCommand(cmd, null);
		}

		public int ExecuteNonQuery(IDbCommand cmd)
		{
			CheckReaders();
			Prepare(cmd);
			return cmd.ExecuteNonQuery();
		}

		public IDataReader ExecuteReader(IDbCommand cmd)
		{
			CheckReaders();
			Prepare(cmd);
			IDataReader reader;
			if (factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders == false)
			{
				reader = new NHybridDataReader(cmd.ExecuteReader());
			}
			else
			{
				reader = cmd.ExecuteReader();
			}

			readersToClose.Add(reader);
			LogOpenReader();
			return reader;
		}

		/// <summary>
		/// Ensures that the Driver's rules for Multiple Open DataReaders are being followed.
		/// </summary>
		protected void CheckReaders()
		{
			// early exit because we don't need to move an open IDataReader into memory
			// since the Driver supports mult open readers.
			if (factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders)
			{
				return;
			}

			foreach (NHybridDataReader reader in readersToClose)
			{
				reader.ReadIntoMemory();
			}
		}

		public void CloseCommand(IDbCommand cmd, IDataReader reader)
		{
			CloseQueryCommand(cmd, reader);
		}

		public void CloseCommands()
		{
			foreach (IDataReader reader in readersToClose)
			{
				try
				{
					LogCloseReader();
					reader.Dispose();
				}
				catch (Exception e)
				{
					log.Warn("Could not close IDataReader", e);
				}
			}
			readersToClose.Clear();

			foreach (IDbCommand cmd in commandsToClose)
			{
				try
				{
					CloseQueryCommand(cmd);
				}
				catch (Exception e)
				{
					// no big deal
					log.Warn("Could not close ADO.NET Command", e);
				}
			}
			commandsToClose.Clear();
		}

		private void CloseQueryCommand(IDbCommand cmd)
		{
			try
			{
				// no equiv to the java code in here
				if (cmd != null)
				{
					cmd.Dispose();
					LogClosePreparedCommand();
				}
			}
			catch (Exception e)
			{
				log.Warn("exception clearing maxRows/queryTimeout", e);
				//cmd.close();  if there was a close method in command
				return; // NOTE: early exit!
			}

			if (lastQuery == cmd)
			{
				lastQuery = null;
			}
		}

		public void CloseQueryCommand(IDbCommand st, IDataReader reader)
		{
			commandsToClose.Remove(st);
			if (reader != null)
			{
				readersToClose.Remove(reader);
			}
			try
			{
				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
					LogCloseReader();
				}
			}
			finally
			{
				CloseQueryCommand(st);
			}
		}

		/// <summary></summary>
		public void ExecuteBatch()
		{
			// if there is currently a command that a batch is
			// being built for then execute it
			if (batchCommand != null)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ps"></param>
		protected abstract void DoExecuteBatch(IDbCommand ps);

		/// <summary>
		/// Adds the expected row count into the batch.
		/// </summary>
		/// <param name="expectedRowCount">The number of rows expected to be affected by the query.</param>
		/// <remarks>
		/// If Batching is not supported, then this is when the Command should be executed.  If Batching
		/// is supported then it should hold of on executing the batch until explicitly told to.
		/// </remarks>
		public abstract void AddToBatch(int expectedRowCount);

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

		private void Log(IDbCommand command)
		{
			if (logSql.IsDebugEnabled || factory.IsShowSqlEnabled)
			{
				string outputText;

				if (command.Parameters.Count == 0)
				{
					outputText = command.CommandText;
				}
				else
				{
					StringBuilder output = new StringBuilder();
					output.Append(command.CommandText);
					output.Append("; ");

					IDataParameter p;
					int count = command.Parameters.Count;
					for (int i = 0; i < count; i++)
					{
						p = (IDataParameter) command.Parameters[i];
						output.Append(string.Format("{0} = '{1}'", p.ParameterName, p.Value));

						if (i + 1 < count)
						{
							output.Append(", ");
						}
					}
					outputText = output.ToString();
				}
				logSql.Debug(outputText);

				if (factory.IsShowSqlEnabled)
				{
					Console.Out.Write("NHibernate: ");
					Console.Out.WriteLine(outputText);
				}
			}
		}

		private static void LogOpenPreparedCommand()
		{
			if (log.IsDebugEnabled)
			{
				openCommandCount++;
				log.Debug("Opened new IDbCommand, open IDbCommands :" + openCommandCount);
			}
		}

		private static void LogClosePreparedCommand()
		{
			if (log.IsDebugEnabled)
			{
				openCommandCount--;
				log.Debug("Closed IDbCommand, open IDbCommands :" + openCommandCount);
			}
		}

		private static void LogOpenReader()
		{
			if (log.IsDebugEnabled)
			{
				openReaderCount++;
				log.Debug("Opened Reader, open Readers :" + openReaderCount);
			}
		}

		private static void LogCloseReader()
		{
			if (log.IsDebugEnabled)
			{
				openReaderCount--;
				log.Debug("Closed Reader, open Readers :" + openReaderCount);
			}
		}

		public void CancelLastQuery()
		{
			try
			{
				if (lastQuery != null)
				{
					lastQuery.Cancel();
				}
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "Could not cancel query");
			}
		}

		protected ADOException Convert(Exception sqlException, string message)
		{
			return ADOExceptionHelper.Convert(sqlException, message);
		}


		protected void ThrowNumberOfRowsAffectedNotMatchExpectedRowCount(int rowsAffected, int rowsExpected)
		{
			throw new HibernateException(string.Format(
											"SQL insert, update or delete failed"
											+ " (expected affected row count: {0}, actual affected row count: {1})."
											+ " Possible causes: the row was modified or deleted by another user,"
											+ " or a trigger is reporting misleading row count.",
											rowsExpected, rowsAffected));
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
			// Don't log in the finalizer, it causes problems
			// if the output stream is finalized before the batcher.
			//log.Debug( "running BatcherImpl.Dispose(false)" );
			Dispose(false);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			log.Debug("running BatcherImpl.Dispose(true)");
			Dispose(true);
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
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the AdoTransaction if we
			// know this call came through Dispose()
			if (isDisposing)
			{
				CloseCommands();
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);

		}
		#endregion
	}
}
