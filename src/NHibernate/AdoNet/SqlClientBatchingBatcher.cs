using System.Data;
using System.Text;
using NHibernate.AdoNet.Util;

namespace NHibernate.AdoNet
{
	using System;

	/// <summary>
	/// Summary description for SqlClientBatchingBatcher.
	/// </summary>
	public class SqlClientBatchingBatcher : AbstractBatcher
	{
		private int batchSize;
		private int totalExpectedRowsAffected;
		private SqlClientSqlCommandSet currentBatch;
		private StringBuilder currentBatchCommandsLog;

		public SqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			batchSize = Factory.Settings.AdoBatchSize;
			currentBatch = new SqlClientSqlCommandSet();
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
			IDbCommand batchUpdate = CurrentCommand;

			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || log.IsDebugEnabled)
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				currentBatchCommandsLog.Append("command ")
					.Append(currentBatch.CountOfCommands)
					.Append(":")
					.AppendLine(lineWithParameters);
			}
			if (log.IsDebugEnabled)
			{
				log.Debug("Adding to batch:" + lineWithParameters);
			}
			currentBatch.Append((System.Data.SqlClient.SqlCommand) batchUpdate);

			if (currentBatch.CountOfCommands >= batchSize)
			{
				ExecuteBatchWithTiming(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(IDbCommand ps)
		{
			log.DebugFormat("Executing batch");
			CheckReaders();
			Prepare(currentBatch.BatchCommand);
			if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
				currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
			}
			
			int rowsAffected = currentBatch.ExecuteNonQuery();

			Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

			currentBatch.Dispose();
			totalExpectedRowsAffected = 0;
			currentBatch = new SqlClientSqlCommandSet();
		}
	}
}