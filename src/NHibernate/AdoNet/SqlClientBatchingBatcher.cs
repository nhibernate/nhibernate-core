using System.Data;
using System.Text;
using NHibernate.AdoNet.Util;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Summary description for SqlClientBatchingBatcher.
	/// </summary>
	internal class SqlClientBatchingBatcher : AbstractBatcher
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
			currentBatchCommandsLog = new StringBuilder();
		}

		public override int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		public override void AddToBatch(IExpectation expectation)
		{
			totalExpectedRowsAffected += expectation.ExpectedRowCount;
			IDbCommand batchUpdate = CurrentCommand;

			if (log.IsDebugEnabled || Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				string lineWithParameters = Factory.Settings.SqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				currentBatchCommandsLog.Append("Batch command: ").AppendLine(lineWithParameters);
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogCommand("Adding to batch:", batchUpdate, FormatStyle.Basic);
				}
				else if (log.IsDebugEnabled)
				{
					log.Debug("Adding to batch:" + lineWithParameters);
				}
				currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);
			}
			if (currentBatch.CountOfCommands >= batchSize)
			{
				DoExecuteBatch(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(IDbCommand ps)
		{
			log.Debug("Executing batch");
			CheckReaders();
			Prepare(currentBatch.BatchCommand);
			if (log.IsDebugEnabled || Factory.Settings.SqlStatementLogger.IsDebugEnabled)
			{
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
					Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
				else if (log.IsDebugEnabled)
					log.Debug(currentBatchCommandsLog.ToString());
				currentBatchCommandsLog = new StringBuilder();
			}
			int rowsAffected = currentBatch.ExecuteNonQuery();

			Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

			currentBatch.Dispose();
			totalExpectedRowsAffected = 0;
			currentBatch = new SqlClientSqlCommandSet();
		}
	}
}