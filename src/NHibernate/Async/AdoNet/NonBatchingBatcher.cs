﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class NonBatchingBatcher : AbstractBatcher
	{

		/// <summary>
		/// Executes the current <see cref="DbCommand"/> and compares the row Count
		/// to the <c>expectedRowCount</c>.
		/// </summary>
		/// <param name="expectation">
		/// The expected number of rows affected by the query.  A value of less than <c>0</c>
		/// indicates that the number of rows to expect is unknown or should not be a factor.
		/// </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <exception cref="HibernateException">
		/// Thrown when there is an expected number of rows to be affected and the
		/// actual number of rows is different.
		/// </exception>
		public override async Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var cmd = CurrentCommand;
			Driver.AdjustCommand(cmd);
			int rowCount = await (ExecuteNonQueryAsync(cmd, cancellationToken)).ConfigureAwait(false);
			expectation.VerifyOutcomeNonBatched(rowCount, cmd);
		}

		/// <summary>
		/// This Batcher implementation does not support batching so this is a no-op call.  The
		/// actual execution of the <see cref="DbCommand"/> is run in the <c>AddToBatch</c> 
		/// method.
		/// </summary>
		/// <param name="ps"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		protected override Task DoExecuteBatchAsync(DbCommand ps, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				DoExecuteBatch(ps);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
