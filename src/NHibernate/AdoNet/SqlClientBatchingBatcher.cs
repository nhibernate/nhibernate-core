using System.Data;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	internal class SqlClientBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new SqlClientBatchingBatcher(connectionManager, interceptor);
		}
	}

	/// <summary>
	/// Summary description for SqlClientBatchingBatcher.
	/// </summary>
	internal class SqlClientBatchingBatcher : AbstractBatcher
	{
		private int batchSize;
		private int totalExpectedRowsAffected;
		private SqlClientSqlCommandSet currentBatch;
		private StringBuilder currentBatchCommandsLog = new StringBuilder();

		public SqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			batchSize = Factory.Settings.AdoBatchSize;
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