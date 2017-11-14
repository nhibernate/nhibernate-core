using System;
using System.Data.Common;
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;
using NHibernate.Util;

namespace NHibernate.AdoNet
{
	public partial class SqlClientBatchingBatcher : AbstractBatcher
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
			_defaultTimeout = PropertiesHelper.GetInt32(Cfg.Environment.CommandTimeout, Cfg.Environment.Properties, -1);

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
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled)
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
					.Append(_currentBatch.CountOfCommands)
					.Append(":")
					.AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled)
			{
				Log.Debug("Adding to batch:" + lineWithParameters);
			}
			_currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);

			if (_currentBatch.CountOfCommands >= _batchSize)
			{
				ExecuteBatchWithTiming(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			try
			{
				Log.DebugFormat("Executing batch");
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

				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected);
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
					if (Log.IsWarnEnabled)
					{
						Log.Warn(e.ToString());
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
				Log.Warn("Exception clearing batch", e);
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
				Log.Warn("Exception closing batcher", e);
			}
		}
	}
}