using System;
using System.Data;
using System.Text;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Manages prepared statements and batching. Class exists to enforce separation of concerns
	/// </summary>
	public abstract class AbstractBatcher : IBatcher
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof(AbstractBatcher));
		protected static readonly ILog logSql = LogManager.GetLogger("NHibernate.SQL");

		private static int openCommandCount;
		private static int openReaderCount;

		private readonly ConnectionManager connectionManager;
		private readonly ISessionFactoryImplementor factory;

		// batchCommand used to be called batchUpdate - that name to me implied that updates
		// were being sent - however this could be just INSERT/DELETE/SELECT SQL statement not
		// just update.  However I haven't seen this being used with read statements...
		private IDbCommand batchCommand;
		private SqlString batchCommandSql;
		private SqlType[] batchCommandParameterTypes;

		private readonly ISet<IDbCommand> commandsToClose = new HashedSet<IDbCommand>();
		private readonly ISet<IDataReader> readersToClose = new HashedSet<IDataReader>();
		private IDbCommand lastQuery;

		private bool releasing;

		private readonly IInterceptor interceptor;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractBatcher"/> class.
		/// </summary>
		/// <param name="connectionManager">The <see cref="ConnectionManager"/> owning this batcher.</param>
		/// <param name="interceptor"></param>
		public AbstractBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			this.connectionManager = connectionManager;
			this.interceptor = interceptor;
			factory = connectionManager.Factory;
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
			SqlString sql = GetSQL(sqlString);

			IDbCommand cmd = factory.ConnectionProvider.Driver.GenerateCommand(type, sql, parameterTypes);
			LogOpenPreparedCommand();
			if (log.IsDebugEnabled)
			{
				log.Debug("Building an IDbCommand object for the SqlString: " + sql);
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
				LogCommand(cmd);
				IDbConnection sessionConnection = connectionManager.GetConnection();

				if (cmd.Connection != null)
				{
					// make sure the commands connection is the same as the Sessions connection
					// these can be different when the session is disconnected and then reconnected
					if (cmd.Connection != sessionConnection)
					{
						cmd.Connection = sessionConnection;
					}
				}
				else
				{
					cmd.Connection = sessionConnection;
				}

				connectionManager.Transaction.Enlist(cmd);
				Driver.PrepareCommand(cmd);
			}
			catch (InvalidOperationException ioe)
			{
				throw new ADOException("While preparing " + cmd.CommandText + " an error occurred", ioe);
			}
		}

		public IDbCommand PrepareBatchCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			/* NH:
			 * The code inside this block was added for a strange behaviour
			 * discovered using Firebird (some times for us is external issue).
			 * The problem is that batchCommandSql as a value, batchCommand is not null
			 * BUT batchCommand.CommandText is null (I don't know who clear it)
			 */
			bool forceCommandRecreate = batchCommand == null || string.IsNullOrEmpty(batchCommand.CommandText);
			/****************************************/
			if (sql.Equals(batchCommandSql) &&
				ArrayHelper.ArrayEquals(parameterTypes, batchCommandParameterTypes) && !forceCommandRecreate)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("reusing command " + batchCommand.CommandText);
				}
			}
			else
			{
				batchCommand = PrepareCommand(type, sql, parameterTypes); // calls ExecuteBatch()
				batchCommandSql = sql;
				batchCommandParameterTypes = parameterTypes;
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
			InvalidateBatchCommand();
			// close the statement closeStatement(cmd)
			if (cmd != null)
			{
				CloseCommand(cmd, null);
			}
		}

		private void InvalidateBatchCommand()
		{
			batchCommand = null;
			batchCommandSql = null;
			batchCommandParameterTypes = null;
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

			IDataReader reader = cmd.ExecuteReader();

			if (!factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders)
			{
				reader = new NHybridDataReader(reader);
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

		public void CloseCommands()
		{
			releasing = true;
			try
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
						CloseCommand(cmd);
					}
					catch (Exception e)
					{
						// no big deal
						log.Warn("Could not close ADO.NET Command", e);
					}
				}
				commandsToClose.Clear();
			}
			finally
			{
				releasing = false;
			}
		}

		private void CloseCommand(IDbCommand cmd)
		{
			try
			{
				// no equiv to the java code in here
				cmd.Dispose();
				LogClosePreparedCommand();
			}
			catch (Exception e)
			{
				log.Warn("exception clearing maxRows/queryTimeout", e);
				return; // NOTE: early exit!
			}
			finally
			{
				if (!releasing)
				{
					connectionManager.AfterStatement();
				}
			}

			if (lastQuery == cmd)
			{
				lastQuery = null;
			}
		}

		public void CloseCommand(IDbCommand st, IDataReader reader)
		{
			commandsToClose.Remove(st);
			try
			{
				CloseReader(reader);
			}
			finally
			{
				CloseCommand(st);
			}
		}

		public void CloseReader(IDataReader reader)
		{
			/* This method was added because PrepareCommand don't really prepare the command
			 * with its connection. 
			 * In some case we need to manage a reader outsite the command scope. 
			 * To do it we need to use the Batcher.ExecuteReader and then we need something
			 * to close the opened reader.
			 */
			// TODO NH: Study a way to use directly IDbCommand.ExecuteReader() outsite the batcher
			// An example of it's use is the management of generated ID.
			if (reader != null)
			{
				ResultSetWrapper rsw = reader as ResultSetWrapper;
				readersToClose.Remove(rsw == null ? reader : rsw.Target);
				CloseDataReader(reader);
			}
		}

		private void CloseDataReader(IDataReader reader)
		{
			reader.Dispose();
			LogCloseReader();
		}

		/// <summary></summary>
		public void ExecuteBatch()
		{
			// if there is currently a command that a batch is
			// being built for then execute it
			if (batchCommand != null)
			{
				IDbCommand ps = batchCommand;
				InvalidateBatchCommand();
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
		/// Gets or sets the size of the batch, this can change dynamically by
		/// calling the session's SetBatchSize.
		/// </summary>
		/// <value>The size of the batch.</value>
		public abstract int BatchSize
		{
			get;
			set;
		}

		/// <summary>
		/// Adds the expected row count into the batch.
		/// </summary>
		/// <param name="expectation">The number of rows expected to be affected by the query.</param>
		/// <remarks>
		/// If Batching is not supported, then this is when the Command should be executed.  If Batching
		/// is supported then it should hold of on executing the batch until explicitly told to.
		/// </remarks>
		public abstract void AddToBatch(IExpectation expectation);

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
		/// Gets the <see cref="ConnectionManager"/> for this batcher.
		/// </summary>
		protected ConnectionManager ConnectionManager
		{
			get { return connectionManager; }
		}

		protected void LogCommand(IDbCommand command)
		{
			if (logSql.IsDebugEnabled || factory.Settings.IsShowSqlEnabled)
			{
				string outputText = GetCommandLogString(command);
				logSql.Debug(outputText);

				if (factory.Settings.IsShowSqlEnabled)
				{
					Console.Out.Write("NHibernate: ");
					Console.Out.WriteLine(outputText);
				}
			}
		}

		protected string GetCommandLogString(IDbCommand command)
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
			return outputText;
		}

		private void LogOpenPreparedCommand()
		{
			if (log.IsDebugEnabled)
			{
				openCommandCount++;
				log.Debug("Opened new IDbCommand, open IDbCommands: " + openCommandCount);
			}

			if (factory.Statistics.IsStatisticsEnabled)
			{
				factory.StatisticsImplementor.PrepareStatement();
			}
		}

		private void LogClosePreparedCommand()
		{
			if (log.IsDebugEnabled)
			{
				openCommandCount--;
				log.Debug("Closed IDbCommand, open IDbCommands: " + openCommandCount);
			}

			if (factory.Statistics.IsStatisticsEnabled)
			{
				factory.StatisticsImplementor.CloseStatement();
			}
		}

		private static void LogOpenReader()
		{
			if (log.IsDebugEnabled)
			{
				openReaderCount++;
				log.Debug("Opened IDataReader, open IDataReaders: " + openReaderCount);
			}
		}

		private static void LogCloseReader()
		{
			if (log.IsDebugEnabled)
			{
				openReaderCount--;
				log.Debug("Closed IDataReader, open IDataReaders :" + openReaderCount);
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

		public bool HasOpenResources
		{
			get { return commandsToClose.Count > 0 || readersToClose.Count > 0; }
		}

		protected ADOException Convert(Exception sqlException, string message)
		{
			return ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqlException, message);
		}

		#region IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Dispose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~AbstractBatcher()
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

		protected SqlString GetSQL(SqlString sql)
		{
			sql = interceptor.OnPrepareStatement(sql);
			if (sql == null || sql.Length == 0)
			{
				throw new AssertionFailure("Interceptor.OnPrepareStatement(SqlString) returned null or empty SqlString.");
			}
			return sql;
		}
	}
}
