#if NETFX
using System;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using NHibernate.Exceptions;

namespace NHibernate.AdoNet
{
	public class SqlClientBatchingBatcher : AbstractBatcher
	{
		private int _batchSize;
		private int _totalExpectedRowsAffected;
		private SqlClientSqlCommandSet _currentBatch;
		private StringBuilder _currentBatchCommandsLog;
		private readonly int _defaultTimeout;

		public SqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
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
		}

		public override int BatchSize
		{
			get { return _batchSize; }
			set { _batchSize = value; }
		}

		protected override int CountOfStatementsInCurrentBatch
		{
			get { return _currentBatch.CountOfCommands; }
		}

		public override void AddToBatch(IExpectation expectation)
		{
			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			var batchUpdate = CurrentCommand;
			Driver.AdjustCommand(batchUpdate);
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
					.Append(_currentBatch.CountOfCommands)
					.Append(":")
					.AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding to batch:{0}", lineWithParameters);
			}
			_currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);

			if (_currentBatch.CountOfCommands >= _batchSize)
			{
				ExecuteBatchWithTiming(batchUpdate);
			}
		}

		public override Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				_totalExpectedRowsAffected += expectation.ExpectedRowCount;
				var batchUpdate = CurrentCommand;
				Driver.AdjustCommand(batchUpdate);
				string lineWithParameters = null;
				var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
				if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
				{
					lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
					var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
					lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
					_currentBatchCommandsLog.Append("command ")
											.Append(_currentBatch.CountOfCommands)
											.Append(":")
											.AppendLine(lineWithParameters);
				}
				if (Log.IsDebugEnabled())
				{
					Log.Debug("Adding to batch:{0}", lineWithParameters);
				}
				_currentBatch.Append((System.Data.SqlClient.SqlCommand) batchUpdate);

				if (_currentBatch.CountOfCommands >= _batchSize)
				{
					return ExecuteBatchWithTimingAsync(batchUpdate, cancellationToken);
				}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			try
			{
				Log.Debug("Executing batch");
				CheckReaders();
				Prepare(_currentBatch.BatchCommand);
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
			try
			{
				Log.Debug("Executing batch");
				await (CheckReadersAsync(cancellationToken)).ConfigureAwait(false);
				await (PrepareAsync(_currentBatch.BatchCommand, cancellationToken)).ConfigureAwait(false);
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

		private SqlClientSqlCommandSet CreateConfiguredBatch()
		{
			var result = new SqlClientSqlCommandSet();
			if (_defaultTimeout > 0)
			{
				try
				{
					result.CommandTimeout = _defaultTimeout;
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
	}
}
#endif
