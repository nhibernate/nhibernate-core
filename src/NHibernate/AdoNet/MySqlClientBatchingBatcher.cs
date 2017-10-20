using System;
using System.Data.Common;
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;

namespace NHibernate.AdoNet
{
	public partial class MySqlClientBatchingBatcher : AbstractBatcher
	{
		private int batchSize;
		private int totalExpectedRowsAffected;
		private MySqlClientSqlCommandSet currentBatch;
		private StringBuilder currentBatchCommandsLog;

		public MySqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			batchSize = Factory.Settings.AdoBatchSize;
			currentBatch = CreateConfiguredBatch();

			//we always create this, because we need to deal with a scenario in which
			//the user change the logging configuration at runtime. Trying to put this
			//behind an if(log.IsDebugEnabled) will cause a null reference exception 
			//at that point.
			currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
		}

		public override int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		protected override int CountOfStatementsInCurrentBatch
		{
			get { return currentBatch.CountOfCommands; }
		}

		public override void AddToBatch(IExpectation expectation)
		{
			totalExpectedRowsAffected += expectation.ExpectedRowCount;
			var batchUpdate = CurrentCommand;
			Prepare(batchUpdate);
			Driver.AdjustCommand(batchUpdate);
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled)
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				currentBatchCommandsLog.Append("command ")
					.Append(currentBatch.CountOfCommands)
					.Append(":")
					.AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled)
			{
				Log.Debug("Adding to batch:" + lineWithParameters);
			}
			currentBatch.Append(batchUpdate);

			if (currentBatch.CountOfCommands >= batchSize)
			{
				DoExecuteBatch(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			try
			{
				Log.DebugFormat("Executing batch");
				CheckReaders();
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
				}

				int rowsAffected;
				try
				{
					rowsAffected = currentBatch.ExecuteNonQuery();
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
				}

				Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);
			}
			finally
			{
				ClearCurrentBatch();
			}
		}

		private MySqlClientSqlCommandSet CreateConfiguredBatch()
		{
			return new MySqlClientSqlCommandSet(batchSize);
		}

		private void ClearCurrentBatch()
		{
			currentBatch.Dispose();
			totalExpectedRowsAffected = 0;
			currentBatch = CreateConfiguredBatch();

			if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			}
		}

		public override void CloseCommands()
		{
			base.CloseCommands();

			try
			{
				ClearCurrentBatch();
			}
			catch (Exception e)
			{
				// Prevent exceptions when clearing the batch from hiding any original exception
				// (We do not know here if this batch closing occurs after a failure or not.)
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
				currentBatch.Dispose();
			}
			catch (Exception e)
			{
				Log.Warn("Exception closing batcher", e);
			}
		}
	}
}