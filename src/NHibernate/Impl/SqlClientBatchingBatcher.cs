#if NET_2_0

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using NHibernate.AdoNet;
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

		public override void AddToBatch(IExpectation expectation)
		{
			totalExpectedRowsAffected += expectation.ExpectedRowCount;
			log.Info("Adding to batch");
			IDbCommand batchUpdate = CurrentCommand;
			LogCommand(batchUpdate);

			currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);
			if (currentBatch.CountOfCommands >= batchSize)
			{
				DoExecuteBatch(batchUpdate);
			}
		}

		protected override void DoExecuteBatch(IDbCommand ps)
		{
			log.Info("Executing batch");
			CheckReaders();
			Prepare(currentBatch.BatchCommand);
			int rowsAffected = currentBatch.ExecuteNonQuery();
			
			Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

			currentBatch.Dispose();
			totalExpectedRowsAffected = 0;
			currentBatch = new SqlClientSqlCommandSet();
		}

	}
}
#endif