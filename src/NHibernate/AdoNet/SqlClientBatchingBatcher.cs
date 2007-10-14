using System.Data;
using System.Text;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Summary description for SqlClientBatchingBatcher.
	/// </summary>
	internal class SqlClientBatchingBatcher : BatcherImpl
	{
		private int batchSize;
		private int totalExpectedRowsAffected;
		private SqlClientSqlCommandSet currentBatch;
		private StringBuilder currentBatchCommandsLog = new StringBuilder();

		public SqlClientBatchingBatcher(ConnectionManager connectionManager)
			: base(connectionManager)
		{
			batchSize = Factory.BatchSize;
			currentBatch = new SqlClientSqlCommandSet();
		}

		public override int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		public override void AddToBatch(IExpectation expectation)
		{
			totalExpectedRowsAffected += expectation.ExpectedRowCount;
			log.Debug("Adding to batch:");
			IDbCommand batchUpdate = CurrentCommand;
			string commandLoggedText = GetCommandLogString(batchUpdate);
			currentBatchCommandsLog.Append("Batch command: ").
				AppendLine(commandLoggedText);
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

			logSql.Debug(currentBatchCommandsLog.ToString());
			currentBatchCommandsLog = new StringBuilder();
			int rowsAffected = currentBatch.ExecuteNonQuery();

			Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

			currentBatch.Dispose();
			totalExpectedRowsAffected = 0;
			currentBatch = new SqlClientSqlCommandSet();
		}
	}
}