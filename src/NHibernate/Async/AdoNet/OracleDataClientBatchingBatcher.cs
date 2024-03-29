﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Driver;
using NHibernate.Exceptions;

namespace NHibernate.AdoNet
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class OracleDataClientBatchingBatcher : AbstractBatcher
	{

		public override Task AddToBatchAsync(IExpectation expectation, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				bool firstOnBatch = true;
				_totalExpectedRowsAffected += expectation.ExpectedRowCount;
				string lineWithParameters = null;
				var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
				if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled())
				{
					lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(CurrentCommand);
					var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
					lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
					_currentBatchCommandsLog.Append("command ")
						.Append(_countOfCommands)
						.Append(":")
						.AppendLine(lineWithParameters);
				}
				if (Log.IsDebugEnabled())
				{
					Log.Debug("Adding to batch:{0}", lineWithParameters);
				}

				if (_currentBatch == null)
				{
					// use first command as the batching command
					_currentBatch = CurrentCommand;
					_parameterValueListHashTable = new Dictionary<string, List<object>>();
					//oracle does not allow array containing all null values
					// so this Dictionary is keeping track if all values are null or not
					_parameterIsAllNullsHashTable = new Dictionary<string, bool>();
				}
				else
				{
					firstOnBatch = false;
				}

				foreach (DbParameter currentParameter in CurrentCommand.Parameters)
				{
					List<object> parameterValueList;
					if (firstOnBatch)
					{
						parameterValueList = new List<object>();
						_parameterValueListHashTable.Add(currentParameter.ParameterName, parameterValueList);
						_parameterIsAllNullsHashTable.Add(currentParameter.ParameterName, true);
					}
					else
					{
						parameterValueList = _parameterValueListHashTable[currentParameter.ParameterName];
					}

					if (currentParameter.Value != DBNull.Value)
					{
						_parameterIsAllNullsHashTable[currentParameter.ParameterName] = false;
					}
					parameterValueList.Add(currentParameter.Value);
				}

				_countOfCommands++;

				if (_countOfCommands >= _batchSize)
				{
					return ExecuteBatchWithTimingAsync(_currentBatch, cancellationToken);
				}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		protected override async Task DoExecuteBatchAsync(DbCommand ps, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (_currentBatch != null)
			{
				int arraySize = 0;
				_countOfCommands = 0;

				Log.Info("Executing batch");
				await (CheckReadersAsync(cancellationToken)).ConfigureAwait(false);
				await (PrepareAsync(_currentBatch, cancellationToken)).ConfigureAwait(false);

				if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
				{
					Factory.Settings.SqlStatementLogger.LogBatchCommand(_currentBatchCommandsLog.ToString());
					_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
				}

				foreach (DbParameter currentParameter in _currentBatch.Parameters)
				{
					List<object> parameterValueArray = _parameterValueListHashTable[currentParameter.ParameterName];
					currentParameter.Value = parameterValueArray.ToArray();
					arraySize = parameterValueArray.Count;
				}

				// setting the ArrayBindCount on the OracleCommand
				// this value is not a part of the ADO.NET API.
				// It's and ODP implementation, so it is being set by reflection
				SetArrayBindCount(arraySize);
				try
				{
					int rowsAffected;
					try
					{
						rowsAffected = await (_currentBatch.ExecuteNonQueryAsync(cancellationToken)).ConfigureAwait(false);
					}
					catch (DbException e)
					{
						throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
					}

					Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected, ps);
				}
				finally
				{
					// Cleaning up even if batched outcome is invalid
					_totalExpectedRowsAffected = 0;
					_currentBatch = null;
					_parameterValueListHashTable = null;
				}
			}
		}
	}
}
