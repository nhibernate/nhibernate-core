using System;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// An implementation of the <see cref="IBatcher" /> 
	/// interface that does no batching.
	/// </summary>
	public partial class NonBatchingBatcher : AbstractBatcher
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NonBatchingBatcher"/> class.
		/// </summary>
		/// <param name="connectionManager">The <see cref="ConnectionManager"/> for this batcher.</param>
		/// <param name="interceptor"></param>
		public NonBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
		}

		/// <summary>
		/// Executes the current <see cref="DbCommand"/> and compares the row Count
		/// to the <c>expectedRowCount</c>.
		/// </summary>
		/// <param name="expectation">
		/// The expected number of rows affected by the query.  A value of less than <c>0</c>
		/// indicates that the number of rows to expect is unknown or should not be a factor.
		/// </param>
		/// <exception cref="HibernateException">
		/// Thrown when there is an expected number of rows to be affected and the
		/// actual number of rows is different.
		/// </exception>
		public override void AddToBatch(IExpectation expectation)
		{
			var cmd = CurrentCommand;
			Driver.AdjustCommand(cmd);
			int rowCount = ExecuteNonQuery(cmd);
			expectation.VerifyOutcomeNonBatched(rowCount, cmd);
		}

		/// <summary>
		/// This Batcher implementation does not support batching so this is a no-op call.  The
		/// actual execution of the <see cref="DbCommand"/> is run in the <c>AddToBatch</c> 
		/// method.
		/// </summary>
		/// <param name="ps"></param>
		protected override void DoExecuteBatch(DbCommand ps)
		{
		}

		protected override int CountOfStatementsInCurrentBatch
		{
			get { return 1; }
		}


		public override int BatchSize
		{
			get { return 1; }
			set { throw new NotSupportedException("No batch size was defined for the session factory, batching is disabled. Set adonet.batch_size = 1 to enable batching."); }
		}
	}
}