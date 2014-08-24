using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace NHibernate.AdoNet
{
	using Action = System.Action;

	public class MySqlClientSqlCommandSet : IDisposable
	{
		private static readonly System.Type adapterType;
		private readonly object instance;
		private readonly Action doInitialise;
		private readonly Action<int> batchSizeSetter;
		private readonly Func<IDbCommand, int> doAppend;
		private readonly Func<int> doExecuteNonQuery;
		private readonly Action doDispose;
		private int countOfCommands;

		static MySqlClientSqlCommandSet()
		{
			var sysData = Assembly.Load("MySql.Data");
			adapterType = sysData.GetType("MySql.Data.MySqlClient.MySqlDataAdapter");
			Debug.Assert(adapterType != null, "Could not find MySqlDataAdapter!");
		}

		public MySqlClientSqlCommandSet(int batchSize)
		{
			instance = Activator.CreateInstance(adapterType, true);
			doInitialise = (Action) Delegate.CreateDelegate(typeof (Action), instance, "InitializeBatching");
			batchSizeSetter = (Action<int>) Delegate.CreateDelegate(typeof (Action<int>), instance, "set_UpdateBatchSize");
			doAppend = (Func<IDbCommand, int>) Delegate.CreateDelegate(typeof (Func<IDbCommand, int>), instance, "AddToBatch");
			doExecuteNonQuery = (Func<int>) Delegate.CreateDelegate(typeof (Func<int>), instance, "ExecuteBatch");
			doDispose = (Action)Delegate.CreateDelegate(typeof(Action), instance, "Dispose");

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
	}
}