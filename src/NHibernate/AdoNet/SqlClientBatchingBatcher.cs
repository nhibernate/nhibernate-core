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
			if(log.IsDebugEnabled)
			{
				currentBatchCommandsLog = new StringBuilder();
			}
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
			if (log.IsDebugEnabled)
			{
				string lineWithParameters = Factory.Settings.SqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogCommand("Adding to batch:", batchUpdate, FormatStyle.Basic);
				}
				else
				{
					log.Debug("Adding to batch:" + lineWithParameters);
				}
				currentBatchCommandsLog.Append("Batch command: ").AppendLine(lineWithParameters);
			}
			else
			{
				Factory.Settings.SqlStatementLogger.LogCommand(batchUpdate, FormatStyle.Basic);
			}
			currentBatch.Append((System.Data.SqlClient.SqlCommand) batchUpdate);
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
			if (log.IsDebugEnabled)
			{
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