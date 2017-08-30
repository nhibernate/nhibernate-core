using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using NHibernate.AdoNet.Util;
using NHibernate.Exceptions;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Summary description for OracleDataClientBatchingBatcher.
	/// By Tomer Avissar
	/// </summary>
	public partial class OracleDataClientBatchingBatcher : AbstractBatcher
	{
		private int _batchSize;
		private int _countOfCommands;
		private int _totalExpectedRowsAffected;
		private DbCommand _currentBatch;
		private IDictionary<string, List<object>> _parameterValueListHashTable;
		private IDictionary<string, bool> _parameterIsAllNullsHashTable;
		private StringBuilder _currentBatchCommandsLog;

		public OracleDataClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			_batchSize = Factory.Settings.AdoBatchSize;
			//we always create this, because we need to deal with a scenario in which
			//the user change the logging configuration at runtime. Trying to put this
			//behind an if(log.IsDebugEnabled) will cause a null reference exception 
			//at that point.
			_currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
		}

		public override void AddToBatch(IExpectation expectation)
		{
			bool firstOnBatch = true;
			_totalExpectedRowsAffected += expectation.ExpectedRowCount;
			string lineWithParameters = null;
			var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
			if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled)
			{
				lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(CurrentCommand);
				var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
				lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
				_currentBatchCommandsLog.Append("command ")
					.Append(_countOfCommands)
					.Append(":")
					.AppendLine(lineWithParameters);
			}
			if (Log.IsDebugEnabled)
			{
				Log.Debug("Adding to batch:" + lineWithParameters);
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
				ExecuteBatchWithTiming(_currentBatch);
			}
		}

		protected override void DoExecuteBatch(DbCommand ps)
		{
			if (_currentBatch != null)
			{
				int arraySize = 0;
				_countOfCommands = 0;

				Log.Info("Executing batch");
				CheckReaders();
				Prepare(_currentBatch);

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
						rowsAffected = _currentBatch.ExecuteNonQuery();
					}
					catch (DbException e)
					{
						throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
					}

					Expectations.VerifyOutcomeBatched(_totalExpectedRowsAffected, rowsAffected);
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

		protected override int CountOfStatementsInCurrentBatch
		{
			get { return _countOfCommands; }
		}

		private void SetArrayBindCount(int arraySize)
		{
			//TODO: cache the property info.
			var objType = _currentBatch.GetType();
			var propInfo = objType.GetProperty("ArrayBindCount");
			propInfo.SetValue(_currentBatch, arraySize, null);
		}

		public override int BatchSize
		{
			get { return _batchSize; }
			set { _batchSize = value; }
		}
	}
}