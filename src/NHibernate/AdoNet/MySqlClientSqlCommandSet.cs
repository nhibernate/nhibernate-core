using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace NHibernate.AdoNet
{
	public class MySqlClientSqlCommandSet : IDisposable
	{
		private static readonly System.Type sqlCmdSetType;
		private readonly object instance;
		private readonly InitialiseCommand doInitialise;
		private readonly PropSetter<int> batchSizeSetter;
		private readonly AppendCommand doAppend;
		private readonly ExecuteNonQueryCommand doExecuteNonQuery;
		private readonly DisposeCommand doDispose;
		private int countOfCommands;

		static MySqlClientSqlCommandSet()
		{
			Assembly sysData = Assembly.Load("MySql.Data");
			sqlCmdSetType = sysData.GetType("MySql.Data.MySqlClient.MySqlDataAdapter");
			Debug.Assert(sqlCmdSetType != null, "Could not find MySqlDataAdapter!");
		}

		public MySqlClientSqlCommandSet(int batchSize)
		{
			instance = Activator.CreateInstance(sqlCmdSetType, true);
			doInitialise = (InitialiseCommand)Delegate.CreateDelegate(typeof(InitialiseCommand), instance, "InitializeBatching");
			batchSizeSetter = (PropSetter<int>)Delegate.CreateDelegate(typeof(PropSetter<int>), instance, "set_UpdateBatchSize");
			doAppend = (AppendCommand)Delegate.CreateDelegate(typeof(AppendCommand), instance, "AddToBatch");
			doExecuteNonQuery = (ExecuteNonQueryCommand)Delegate.CreateDelegate(typeof(ExecuteNonQueryCommand), instance, "ExecuteBatch");
			doDispose = (DisposeCommand)Delegate.CreateDelegate(typeof(DisposeCommand), instance, "Dispose");

			Initialise(batchSize);
		}

		private void Initialise(int batchSize)
		{
			doInitialise();
			batchSizeSetter(batchSize);
		}

		public void Append(IDbCommand command)
		{
			doAppend(command);
			countOfCommands++;
		}

		public void Dispose()
		{
			doDispose();
		}

		public int ExecuteNonQuery()
		{
			try
			{
				if (CountOfCommands == 0)
				{
					return 0;
				}

				return doExecuteNonQuery();
			}
			catch (Exception exception)
			{
				throw new HibernateException("An exception occured when executing batch queries", exception);
			}
		}

		public int CountOfCommands
		{
			get
			{
				return countOfCommands;
			}
		}

		#region Delegate Definations

		private delegate void PropSetter<T>(T item);

		private delegate void InitialiseCommand();

		private delegate int AppendCommand(IDbCommand command);

		private delegate int ExecuteNonQueryCommand();

		private delegate void DisposeCommand();

		#endregion
	}
}