using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NHibernate.AdoNet.Util;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Manages prepared statements and batching. Class exists to enforce separation of concerns
	/// </summary>
	public abstract partial class AbstractBatcher : IBatcher
	{
		protected static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(AbstractBatcher));

		private static int _openCommandCount;
		private static int _openReaderCount;

		private readonly ConnectionManager _connectionManager;
		private readonly ISessionFactoryImplementor _factory;
		private readonly IInterceptor _interceptor;

		// batchCommand used to be called batchUpdate - that name to me implied that updates
		// were being sent - however this could be just INSERT/DELETE/SELECT SQL statement not
		// just update.  However I haven't seen this being used with read statements...
		private DbCommand _batchCommand;
		private SqlString _batchCommandSql;
		private SqlType[] _batchCommandParameterTypes;
		private readonly HashSet<DbCommand> _commandsToClose = new HashSet<DbCommand>();
		private readonly HashSet<DbDataReader> _readersToClose = new HashSet<DbDataReader>();
		private readonly IDictionary<DbDataReader, Stopwatch> _readersDuration = new Dictionary<DbDataReader, Stopwatch>();
		private DbCommand _lastQuery;
		private bool _releasing;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractBatcher"/> class.
		/// </summary>
		/// <param name="connectionManager">The <see cref="ConnectionManager"/> owning this batcher.</param>
		/// <param name="interceptor"></param>
		protected AbstractBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			_connectionManager = connectionManager;
			_interceptor = interceptor;
			_factory = connectionManager.Factory;
		}

		protected IDriver Driver
		{
			get { return _factory.ConnectionProvider.Driver; }
		}

		/// <summary>
		/// Gets the current <see cref="DbCommand"/> that is contained for this Batch
		/// </summary>
		/// <value>The current <see cref="DbCommand"/>.</value>
		protected DbCommand CurrentCommand
		{
			get { return _batchCommand; }
		}

		public DbCommand Generate(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			SqlString sql = GetSQL(sqlString);

			var cmd = _factory.ConnectionProvider.Driver.GenerateCommand(type, sql, parameterTypes);
			LogOpenPreparedCommand();
			if (Log.IsDebugEnabled)
			{
				Log.Debug("Building an DbCommand object for the SqlString: " + sql);
			}
			_commandsToClose.Add(cmd);
			return cmd;
		}

		/// <summary>
		/// Prepares the <see cref="DbCommand"/> for execution in the database.
		/// </summary>
		/// <remarks>
		/// This takes care of hooking the <see cref="DbCommand"/> up to an <see cref="DbConnection"/>
		/// and <see cref="DbTransaction"/> if one exists.  It will call <c>Prepare</c> if the Driver
		/// supports preparing commands.
		/// </remarks>
		protected void Prepare(DbCommand cmd)
		{
			try
			{
				var sessionConnection = _connectionManager.GetConnection();

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

				_connectionManager.Transaction.Enlist(cmd);
				Driver.PrepareCommand(cmd);
			}
			catch (InvalidOperationException ioe)
			{
				throw new ADOException("While preparing " + cmd.CommandText + " an error occurred", ioe);
			}
		}

		public virtual DbCommand PrepareBatchCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			if (sql.Equals(_batchCommandSql) && ArrayHelper.ArrayEquals(parameterTypes, _batchCommandParameterTypes))
			{
				if (Log.IsDebugEnabled)
				{
					Log.Debug("reusing command " + _batchCommand.CommandText);
				}
			}
			else
			{
				_batchCommand = PrepareCommand(type, sql, parameterTypes); // calls ExecuteBatch()
				_batchCommandSql = sql;
				_batchCommandParameterTypes = parameterTypes;
			}

			return _batchCommand;
		}

		public DbCommand PrepareCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			OnPreparedCommand();

			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			return Generate(type, sql, parameterTypes);
		}

		protected virtual void OnPreparedCommand()
		{
			// a new DbCommand is being prepared and a new (potential) batch
			// started - so execute the current batch of commands.
			ExecuteBatch();
		}

		public DbCommand PrepareQueryCommand(CommandType type, SqlString sql, SqlType[] parameterTypes)
		{
			// do not actually prepare the Command here - instead just generate it because
			// if the command is associated with an ADO.NET Transaction/Connection while
			// another open one Command is doing something then an exception will be 
			// thrown.
			var command = Generate(type, sql, parameterTypes);
			_lastQuery = command;
			return command;
		}

		public void AbortBatch(Exception e)
		{
			var cmd = _batchCommand;
			InvalidateBatchCommand();
			// close the statement closeStatement(cmd)
			if (cmd != null)
			{
				CloseCommand(cmd, null);
			}
		}

		private void InvalidateBatchCommand()
		{
			_batchCommand = null;
			_batchCommandSql = null;
			_batchCommandParameterTypes = null;
		}

		public int ExecuteNonQuery(DbCommand cmd)
		{
			CheckReaders();
			LogCommand(cmd);
			Prepare(cmd);
			Stopwatch duration = null;
			if (Log.IsDebugEnabled)
				duration = Stopwatch.StartNew();
			try
			{
				return cmd.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				e.Data["actual-sql-query"] = cmd.CommandText;
				Log.Error("Could not execute command: " + cmd.CommandText, e);
				throw;
			}
			finally
			{
				if (Log.IsDebugEnabled && duration != null)
					Log.DebugFormat("ExecuteNonQuery took {0} ms", duration.ElapsedMilliseconds);
			}
		}

		public virtual DbDataReader ExecuteReader(DbCommand cmd)
		{
			CheckReaders();
			LogCommand(cmd);
			Prepare(cmd);
			Stopwatch duration = null;
			if (Log.IsDebugEnabled)
				duration = Stopwatch.StartNew();
			DbDataReader reader = null;
			try
			{
				reader = cmd.ExecuteReader();
			}
			catch (Exception e)
			{
				e.Data["actual-sql-query"] = cmd.CommandText;
				Log.Error("Could not execute query: " + cmd.CommandText, e);
				throw;
			}
			finally
			{
				if (Log.IsDebugEnabled && duration != null && reader != null)
				{
					Log.DebugFormat("ExecuteReader took {0} ms", duration.ElapsedMilliseconds);
					_readersDuration[reader] = duration;
				}
			}

			if (!_factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders)
			{
				reader = NHybridDataReader.Create(reader);
			}

			_readersToClose.Add(reader);
			LogOpenReader();
			return reader;
		}

		/// <summary>
		/// Ensures that the Driver's rules for Multiple Open DataReaders are being followed.
		/// </summary>
		protected void CheckReaders()
		{
			// early exit because we don't need to move an open DbDataReader into memory
			// since the Driver supports mult open readers.
			if (_factory.ConnectionProvider.Driver.SupportsMultipleOpenReaders)
			{
				return;
			}

			foreach (NHybridDataReader reader in _readersToClose)
			{
				reader.ReadIntoMemory();
			}
		}

		public virtual void CloseCommands()
		{
			_releasing = true;
			try
			{
				foreach (var reader in new HashSet<DbDataReader>(_readersToClose))
				{
					try
					{
						CloseReader(reader);
					}
					catch (Exception e)
					{
						Log.Warn("Could not close DbDataReader", e);
					}
				}

				foreach (var cmd in _commandsToClose)
				{
					try
					{
						CloseCommand(cmd);
					}
					catch (Exception e)
					{
						// no big deal
						Log.Warn("Could not close ADO.NET Command", e);
					}
				}
				_commandsToClose.Clear();
			}
			finally
			{
				_releasing = false;
			}
		}

		private void CloseCommand(DbCommand cmd)
		{
			try
			{
				// no equiv to the java code in here
				cmd.Dispose();
				LogClosePreparedCommand();
			}
			catch (Exception e)
			{
				Log.Warn("exception clearing maxRows/queryTimeout", e);
				return; // NOTE: early exit!
			}
			finally
			{
				if (!_releasing)
				{
					_connectionManager.AfterStatement();
				}
			}

			if (_lastQuery == cmd)
			{
				_lastQuery = null;
			}
		}

		public void CloseCommand(DbCommand st, DbDataReader reader)
		{
			_commandsToClose.Remove(st);
			try
			{
				CloseReader(reader);
			}
			finally
			{
				CloseCommand(st);
			}
		}

		public void CloseReader(DbDataReader reader)
		{
			/* This method was added because PrepareCommand don't really prepare the command
			 * with its connection. 
			 * In some case we need to manage a reader outsite the command scope. 
			 * To do it we need to use the Batcher.ExecuteReader and then we need something
			 * to close the opened reader.
			 */
			// TODO NH: Study a way to use directly DbCommand.ExecuteReader() outsite the batcher
			// An example of it's use is the management of generated ID.
			if (reader == null)
				return;

			var rsw = reader as ResultSetWrapper;
			var actualReader = rsw == null ? reader : rsw.Target;
			_readersToClose.Remove(actualReader);

			var duration = GetReaderStopwatch(actualReader);

			try
			{
				reader.Dispose();
			}
			catch (Exception e)
			{
				// NH2205 - prevent exceptions when closing the reader from hiding any original exception
				Log.Warn("exception closing reader", e);
			}

			LogCloseReader();
			LogDuration(duration);
		}

		private Stopwatch GetReaderStopwatch(DbDataReader reader)
		{
			var nhReader = reader as NHybridDataReader;
			var actualReader = nhReader == null ? reader : nhReader.Target;

			Stopwatch duration;
			if (_readersDuration.TryGetValue(actualReader, out duration))
				_readersDuration.Remove(actualReader);
			return duration;
		}

		private static void LogDuration(Stopwatch duration)
		{
			if (!Log.IsDebugEnabled || duration == null) return;

			Log.DebugFormat("DataReader was closed after {0} ms", duration.ElapsedMilliseconds);
		}

		public void ExecuteBatch()
		{
			// if there is currently a command that a batch is
			// being built for then execute it
			if (_batchCommand != null)
			{
				var ps = _batchCommand;
				InvalidateBatchCommand();
				try
				{
					ExecuteBatchWithTiming(ps);
				}
				finally
				{
					CloseCommand(ps, null);
				}
			}
		}

		protected void ExecuteBatchWithTiming(DbCommand ps)
		{
			Stopwatch duration = null;
			if (Log.IsDebugEnabled)
				duration = Stopwatch.StartNew();
			var countBeforeExecutingBatch = CountOfStatementsInCurrentBatch;
			DoExecuteBatch(ps);
			if (Log.IsDebugEnabled && duration != null)
				Log.DebugFormat("ExecuteBatch for {0} statements took {1} ms",
					countBeforeExecutingBatch,
					duration.ElapsedMilliseconds);
		}

		protected abstract void DoExecuteBatch(DbCommand ps);

		protected abstract int CountOfStatementsInCurrentBatch { get; }

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
			get { return _factory; }
		}

		/// <summary>
		/// Gets the <see cref="ConnectionManager"/> for this batcher.
		/// </summary>
		protected ConnectionManager ConnectionManager
		{
			get { return _connectionManager; }
		}

		protected void LogCommand(DbCommand command)
		{
			_factory.Settings.SqlStatementLogger.LogCommand(command, FormatStyle.Basic);
		}

		private void LogOpenPreparedCommand()
		{
			if (Log.IsDebugEnabled)
			{
				int currentOpenCommandCount = Interlocked.Increment(ref _openCommandCount);
				Log.Debug("Opened new DbCommand, open DbCommands: " + currentOpenCommandCount);
			}

			if (_factory.Statistics.IsStatisticsEnabled)
			{
				_factory.StatisticsImplementor.PrepareStatement();
			}
		}

		private void LogClosePreparedCommand()
		{
			if (Log.IsDebugEnabled)
			{
				int currentOpenCommandCount = Interlocked.Decrement(ref _openCommandCount);
				Log.Debug("Closed DbCommand, open DbCommands: " + currentOpenCommandCount);
			}

			if (_factory.Statistics.IsStatisticsEnabled)
			{
				_factory.StatisticsImplementor.CloseStatement();
			}
		}

		private static void LogOpenReader()
		{
			if (Log.IsDebugEnabled)
			{
				int currentOpenReaderCount = Interlocked.Increment(ref _openReaderCount);
				Log.Debug("Opened DbDataReader, open DbDataReaders: " + currentOpenReaderCount);
			}
		}

		private static void LogCloseReader()
		{
			if (Log.IsDebugEnabled)
			{
				int currentOpenReaderCount = Interlocked.Decrement(ref _openReaderCount);
				Log.Debug("Closed DbDataReader, open DbDataReaders :" + currentOpenReaderCount);
			}
		}

		public void CancelLastQuery()
		{
			try
			{
				if (_lastQuery != null)
				{
					_lastQuery.Cancel();
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
			get { return _commandsToClose.Count > 0 || _readersToClose.Count > 0; }
		}

		protected Exception Convert(Exception sqlException, string message)
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
			Log.Debug("running BatcherImpl.Dispose(true)");
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
			sql = _interceptor.OnPrepareStatement(sql);
			if (sql == null || sql.Length == 0)
			{
				throw new AssertionFailure("Interceptor.OnPrepareStatement(SqlString) returned null or empty SqlString.");
			}
			return sql;
		}
	}
}