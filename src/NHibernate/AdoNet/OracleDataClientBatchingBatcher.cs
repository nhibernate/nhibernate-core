using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using NHibernate.AdoNet.Util;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Summary description for OracleDataClientBatchingBatcher.
	/// By Tomer Avissar
	/// </summary>
	public class OracleDataClientBatchingBatcher : AbstractBatcher
	{
		private int batchSize;
		private int countOfCommands = 0;
		private int totalExpectedRowsAffected;
		private IDbCommand currentBatch;
		private IDictionary<string, List<object>> parameterValueListHashTable;
		private IDictionary<string, bool> parameterIsAllNullsHashTable;
        private StringBuilder currentBatchCommandsLog;


		public OracleDataClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			batchSize = Factory.Settings.AdoBatchSize;
            //we always create this, because we need to deal with a scenario in which
            //the user change the logging configuration at runtime. Trying to put this
            //behind an if(log.IsDebugEnabled) will cause a null reference exception 
            //at that point.
            currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
        }

		public override void AddToBatch(IExpectation expectation)
		{
			bool firstOnBatch = true;
			totalExpectedRowsAffected += expectation.ExpectedRowCount;
            string lineWithParameters = null;
            var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
            if (sqlStatementLogger.IsDebugEnabled || log.IsDebugEnabled)
            {
                lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(CurrentCommand);
                var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
                lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
                currentBatchCommandsLog.Append("command ")
                    .Append(countOfCommands)
                    .Append(":")
                    .AppendLine(lineWithParameters);
            }
            if (log.IsDebugEnabled)
            {
                log.Debug("Adding to batch:" + lineWithParameters);
            }

			if (currentBatch == null)
			{
				// use first command as the batching command
				currentBatch = CurrentCommand;
				parameterValueListHashTable = new Dictionary<string, List<object>>();
				//oracle does not allow array containing all null values
				// so this Dictionary is keeping track if all values are null or not
				parameterIsAllNullsHashTable = new Dictionary<string, bool>();
			}
			else
			{
				firstOnBatch = false;
			}

			List<object> parameterValueList;
			foreach (IDataParameter currentParameter in CurrentCommand.Parameters)
			{                
				if (firstOnBatch)
				{
					parameterValueList = new List<object>();
					parameterValueListHashTable.Add(currentParameter.ParameterName, parameterValueList);
					parameterIsAllNullsHashTable.Add(currentParameter.ParameterName, true);
				}
				else
				{
					parameterValueList = parameterValueListHashTable[currentParameter.ParameterName];
				}

				if (currentParameter.Value != DBNull.Value)
				{
					parameterIsAllNullsHashTable[currentParameter.ParameterName] = false;
				}
				parameterValueList.Add(currentParameter.Value);
			} 
			
			countOfCommands++;

			if (countOfCommands >= batchSize)
			{
				ExecuteBatchWithTiming(currentBatch);
			}
		}

		protected override void DoExecuteBatch(IDbCommand ps)
		{
			if (currentBatch != null)
			{
				int arraySize = 0;
				countOfCommands = 0;
			   
				log.Info("Executing batch");
				CheckReaders();
				Prepare(currentBatch);

                if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
                {
                    Factory.Settings.SqlStatementLogger.LogBatchCommand(currentBatchCommandsLog.ToString());
                    currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
                }

				foreach (IDataParameter currentParameter in currentBatch.Parameters)
				{
					List<object> parameterValueArray = parameterValueListHashTable[currentParameter.ParameterName];
					currentParameter.Value = parameterValueArray.ToArray();
					arraySize = parameterValueArray.Count;
				}

				// setting the ArrayBindCount on the OracleCommand
				// this value is not a part of the ADO.NET API.
				// It's and ODP implementation, so it is being set by reflection
				SetObjectParam(currentBatch, "ArrayBindCount", arraySize);
				int rowsAffected = currentBatch.ExecuteNonQuery();

				Expectations.VerifyOutcomeBatched(totalExpectedRowsAffected, rowsAffected);

				totalExpectedRowsAffected = 0;
				currentBatch = null;
				parameterValueListHashTable = null; 
			}
		}

		protected override int CountOfStatementsInCurrentBatch
		{
			get { return countOfCommands; }
		}

		private void SetObjectParam(Object obj, string paramName, object paramValue)
		{
			System.Type objType = obj.GetType();
			PropertyInfo propInfo = objType.GetProperty(paramName);
			propInfo.SetValue(obj, paramValue, null);
		}

		public override int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}
	}
}