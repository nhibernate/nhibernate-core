using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

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


		public OracleDataClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			: base(connectionManager, interceptor)
		{
			batchSize = Factory.Settings.AdoBatchSize;
		}

		public override void AddToBatch(IExpectation expectation)
		{
			bool firstOnBatch = true;
			totalExpectedRowsAffected += expectation.ExpectedRowCount;
			log.Info("Adding to batch");
			LogCommand(CurrentCommand);

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