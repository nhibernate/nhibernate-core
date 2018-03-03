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
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Dialect;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;

namespace NHibernate.AdoNet
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class GenericBatchingBatcher : AbstractBatcher
	{

		public override async Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var batchUpdate = CurrentCommand;
			if (_currentBatch.CountOfParameters + CurrentCommand.Parameters.Count > _maxNumberOfParameters)
			{
				await (ExecuteBatchWithTimingAsync(batchUpdate, cancellationToken)).ConfigureAwait(false);
			}
			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			Driver.AdjustCommand(batchUpdate);
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
				                        .Append(_currentBatch.CountOfCommands)
				                        .Append(":")
				                        .AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding to batch:{0}", lineWithParameters);
			}
			
			_currentBatch.Append(CurrentCommand.Parameters);

			if (_currentBatch.CountOfCommands >= BatchSize)
			{
				await (ExecuteBatchWithTimingAsync(batchUpdate, cancellationToken)).ConfigureAwait(false);
			}
		}

		protected override async Task DoExecuteBatchAsync(DbCommand ps, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (_currentBatch.CountOfCommands == 0)
			{
				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, 0);
				return;
			}
			try
			{
				Log.Debug("Executing batch");
				await (CheckReadersAsync(cancellationToken)).ConfigureAwait(false);
				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
				}

				int rowsAffected;
				try
				{
					rowsAffected = await (_currentBatch.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
				}
				catch (DbException e)
				{
					throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
				}

				Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected);
			}
			finally
			{
				ClearCurrentBatch();
			}
		}

		private partial class BatchingCommandSet : IDisposable
		{

			public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (_batchCommand == null)
				{
					return 0;
				}
				await (_batcher.PrepareAsync(_batchCommand, cancellationToken)).ConfigureAwait(false);
				return await (_batchCommand.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
			}
		}
	}
}
