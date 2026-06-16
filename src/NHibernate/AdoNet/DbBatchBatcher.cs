#if NET6_0_OR_GREATER
using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using NHibernate.Exceptions;

namespace NHibernate.AdoNet
{
	public class DbBatchBatcher : AbstractBatcher
	{
		private int _batchSize;
		private int _totalExpectedRowsAffected;
		private DbBatch _currentBatch;
		private StringBuilder _currentBatchCommandsLog;
		private readonly int _defaultTimeout;
		private readonly ConnectionManager _connectionManager;

		public DbBatchBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			_batchSize = Factory.Settings.AdoBatchSize;
			_defaultTimeout = Driver.GetCommandTimeout();

			_currentBatch = CreateConfiguredBatch();
			//we always create this, because we need to deal with a scenario in which
			//the user change the logging configuration at runtime. Trying to put this
			//behind an if(log.IsDebugEnabled) will cause a null reference exception 
			//at that point.
			_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			_connectionManager = connectionManager;
		}

		public override int BatchSize
		{
			get => _batchSize;
			set => _batchSize = value;
		}

		protected override int CountOfStatementsInCurrentBatch => _currentBatch.BatchCommands.Count;

		private void LogBatchCommand(DbCommand batchUpdate)
		{
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
				                        .Append(_currentBatch.BatchCommands.Count)
				                        .Append(':')
				                        .AppendLine(lineWithParameters);
			}

			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding to batch:{0}", lineWithParameters);
			}
		}

		private void AddCommandToBatch(DbCommand batchUpdate)
		{
			var dbBatchCommand = Driver.CreateDbBatchCommandFromDbCommand(_currentBatch, batchUpdate);
			
			_currentBatch.BatchCommands.Add(dbBatchCommand);
		}

		public override void AddToBatch(IExpectation expectation)
		{
			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			var batchUpdate = CurrentCommand;
			Driver.AdjustCommand(batchUpdate);
			LogBatchCommand(batchUpdate);
			AddCommandToBatch(batchUpdate);

			if (CountOfStatementsInCurrentBatch >= _batchSize)
			{
				ExecuteBatchWithTiming(batchUpdate);
			}
		}

		public override Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}

			try
			{
				_totalExpectedRowsAffected += expectation.ExpectedRowCount;
				var batchUpdate = CurrentCommand;
				Driver.AdjustCommand(batchUpdate);
				LogBatchCommand(batchUpdate);
				AddCommandToBatch(batchUpdate);

				if (CountOfStatementsInCurrentBatch >= _batchSize)
				{
					return ExecuteBatchWithTimingAsync(batchUpdate, cancellationToken);
				}

				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException(ex);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			if (_currentBatch.BatchCommands.Count == 0)
			{
				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, 0, ps);
				return;
			}

			try
			{
				Log.Debug("Executing batch");
				CheckReaders();
				Prepare(_currentBatch);
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
				}
				int rowsAffected;
				try
				{
					rowsAffected = _currentBatch.ExecuteNonQuery();
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
				}

				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected, ps);
			}
			finally
			{
				ClearCurrentBatch();
			}
		}

		protected override async Task DoExecuteBatchAsync(DbCommand ps, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (_currentBatch.BatchCommands.Count == 0)
			{
				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, 0, ps);
				return;
			}

			try
			{
				Log.Debug("Executing batch");
				await CheckReadersAsync(cancellationToken).ConfigureAwait(false);
				await PrepareAsync(_currentBatch, cancellationToken).ConfigureAwait(false);
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
				}
				int rowsAffected;
				try
				{
					rowsAffected = await _currentBatch.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
				}

				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected, ps);
			}
			finally
			{
				ClearCurrentBatch();
			}
		}

		private DbBatch CreateConfiguredBatch()
		{
			var result = Driver.CreateBatch();
			if (_defaultTimeout > 0)
			{
				try
				{
					result.Timeout = _defaultTimeout;
				}
				catch (Exception e)
				{
					if (Log.IsWarnEnabled())
					{
						Log.Warn(e, e.ToString());
					}
				}
			}

			return result;
		}

		private void ClearCurrentBatch()
		{
			_currentBatch.Dispose();
			_totalExpectedRowsAffected = 0;
			_currentBatch = CreateConfiguredBatch();

			if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			}
		}

		public override void CloseCommands()
		{
			base.CloseCommands();

			// Prevent exceptions when closing the batch from hiding any original exception
			// (We do not know here if this batch closing occurs after a failure or not.)
			try
			{
				ClearCurrentBatch();
			}
			catch (Exception e)
			{
				Log.Warn(e, "Exception clearing batch");
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			// Prevent exceptions when closing the batch from hiding any original exception
			// (We do not know here if this batch closing occurs after a failure or not.)
			try
			{
				_currentBatch.Dispose();
			}
			catch (Exception e)
			{
				Log.Warn(e, "Exception closing batcher");
			}
		}

		/// <summary>
		/// Prepares the <see cref="DbBatch"/> for execution in the database.
		/// </summary>
		/// <remarks>
		/// This takes care of hooking the <see cref="DbBatch"/> up to an <see cref="DbConnection"/>
		/// and <see cref="DbTransaction"/> if one exists.  It will call <c>Prepare</c> if the Driver
		/// supports preparing batches.
		/// </remarks>
		protected void Prepare(DbBatch batch)
		{
			try
			{
				var sessionConnection = _connectionManager.GetConnection();

				if (batch.Connection != null)
				{
					// make sure the commands connection is the same as the Sessions connection
					// these can be different when the session is disconnected and then reconnected
					if (batch.Connection != sessionConnection)
					{
						batch.Connection = sessionConnection;
					}
				}
				else
				{
					batch.Connection = sessionConnection;
				}

				_connectionManager.EnlistInTransaction(batch);
				Driver.PrepareBatch(batch);
			}
			catch (InvalidOperationException ioe)
			{
				throw new ADOException("While preparing " + string.Join(Environment.NewLine, batch.BatchCommands.Select(x => x.CommandText)) + " an error occurred", ioe);
			}
		}

		/// <summary>
		/// Prepares the <see cref="DbBatch"/> for execution in the database.
		/// </summary>
		/// <remarks>
		/// This takes care of hooking the <see cref="DbBatch"/> up to an <see cref="DbConnection"/>
		/// and <see cref="DbTransaction"/> if one exists.  It will call <c>Prepare</c> if the Driver
		/// supports preparing batches.
		/// </remarks>
		protected async Task PrepareAsync(DbBatch batch, CancellationToken cancellationToken)
		{
			try
			{
				var sessionConnection = await _connectionManager.GetConnectionAsync(cancellationToken).ConfigureAwait(false);

				if (batch.Connection != null)
				{
					// make sure the commands connection is the same as the Sessions connection
					// these can be different when the session is disconnected and then reconnected
					if (batch.Connection != sessionConnection)
					{
						batch.Connection = sessionConnection;
					}
				}
				else
				{
					batch.Connection = sessionConnection;
				}

				_connectionManager.EnlistInTransaction(batch);
				Driver.PrepareBatch(batch);
			}
			catch (InvalidOperationException ioe)
			{
				throw new ADOException("While preparing " + string.Join(Environment.NewLine, batch.BatchCommands.Select(x => x.CommandText)) + " an error occurred", ioe);
			}
		}
	}
}
#endif
