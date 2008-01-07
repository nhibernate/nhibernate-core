using System;
using System.Collections;
using System.Data;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.AdoNet
{
	internal class OracleDataClientClientBatchingBatcherFactory : IBatcherFactory
	{
		public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
		{
			return new OracleDataClientBatchingBatcher(connectionManager, interceptor);
		}
	}

	/// <summary>
	/// Summary description for OracleDataClientBatchingBatcher.
	/// By Tomer Avissar
	/// </summary>
	internal class OracleDataClientBatchingBatcher : AbstractBatcher
	{
		private int batchSize;
		private int countOfCommands = 0;
		private int totalExpectedRowsAffected;
		private IDbCommand currentBatch;
		private Hashtable parameterValueArrayHashTable;
		private Hashtable parameterIsAllNullsHashTable;


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
				parameterValueArrayHashTable = new Hashtable();
				//oracle does not allow array containing all null values
				// so this HashTable is keeping track if all values are null or not
				parameterIsAllNullsHashTable = new Hashtable();
			}
			else
			{
				firstOnBatch = false;
			}

			ArrayList parameterValueArray;
			foreach (IDataParameter currentParameter in CurrentCommand.Parameters)
			{                
				if (firstOnBatch)
				{
					parameterValueArray = new ArrayList();
					parameterValueArrayHashTable.Add(currentParameter.ParameterName, parameterValueArray);
					parameterIsAllNullsHashTable.Add(currentParameter.ParameterName, true);
				}
				else
				{
					parameterValueArray = parameterValueArrayHashTable[currentParameter.ParameterName] as ArrayList;
				}

				if (currentParameter.Value != DBNull.Value)
				{
					parameterIsAllNullsHashTable[currentParameter.ParameterName] = false;
				}
				parameterValueArray.Add(currentParameter.Value);
			} 
			
			countOfCommands++;

			if (countOfCommands >= batchSize)
			{
				DoExecuteBatch(currentBatch);
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
					ArrayList parameterValueArray = parameterValueArrayHashTable[currentParameter.ParameterName] as ArrayList;
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
				parameterValueArrayHashTable = null; 
			}
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