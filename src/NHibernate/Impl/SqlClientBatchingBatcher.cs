#if NET_2_0

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// Summary description for SqlClientBatchingBatcher.
	/// </summary>
	internal class SqlClientBatchingBatcher : BatcherImpl
	{
		private int batchSize;
		private int totalExpectedRowsAffected;
		bool checkExpectedRows = true;
		SqlClientSqlCommandSet currentBatch;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		public SqlClientBatchingBatcher(ISessionImplementor session)
			: base(session)
		{
			batchSize = Factory.BatchSize;
			currentBatch = new SqlClientSqlCommandSet();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expectedRowCount"></param>
		public override void AddToBatch(int expectedRowCount)
		{
			// negative here means that we don't know what to expect
			// because we don't get a result per query in the batch (like in Java)
			// we need to turn off the expected row count for the whole batch.
			if (expectedRowCount < 0)
				checkExpectedRows = false;
			totalExpectedRowsAffected += expectedRowCount;
			log.Info("Adding to batch");
			IDbCommand batchUpdate = CurrentCommand;
			currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);
			if (currentBatch.CountOfCommands > batchSize)
			{
				DoExecuteBatch(batchUpdate);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ps"></param>
		protected override void DoExecuteBatch(IDbCommand ps)
		{
			log.Info("Executing batch");
			CheckReaders();
			Prepare(currentBatch.BatchCommand);
			int rowsAffected = currentBatch.ExecuteNonQuery();
			if (checkExpectedRows && rowsAffected != totalExpectedRowsAffected)
			{
				ThrowNumberOfRowsAffectedNotMatchExpectedRowCount(rowsAffected, totalExpectedRowsAffected);
			}
			currentBatch.Dispose();
			checkExpectedRows = true;
			totalExpectedRowsAffected = 0;

			currentBatch = new SqlClientSqlCommandSet();
		}

	}
}
#endif