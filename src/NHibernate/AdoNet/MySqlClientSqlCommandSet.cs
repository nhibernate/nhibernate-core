using System;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.AdoNet
{
	public class MySqlClientSqlCommandSet : IDisposable
	{
		private static readonly System.Type adapterType;
		private static readonly Action<object> doInitialise;
		private static readonly Action<object, int> batchSizeSetter;
		private static readonly Action<object, DbCommand> doAppend;
		private static readonly Func<object, int> doExecuteNonQuery;
		private static readonly Action<object> doDispose;

		private readonly object instance;
		private int countOfCommands;

		static MySqlClientSqlCommandSet()
		{
			var sysData = Assembly.Load("MySql.Data");
			adapterType = sysData.GetType("MySql.Data.MySqlClient.MySqlDataAdapter");
			Debug.Assert(adapterType != null, "Could not find MySqlDataAdapter!");

			doInitialise = DelegateHelper.BuildAction(adapterType, "InitializeBatching");
			batchSizeSetter = DelegateHelper.BuildPropertySetter<int>(adapterType, "UpdateBatchSize");
			doAppend = DelegateHelper.BuildAction<DbCommand>(adapterType, "AddToBatch");
			doExecuteNonQuery = DelegateHelper.BuildFunc<int>(adapterType, "ExecuteBatch");
			doDispose = DelegateHelper.BuildAction(adapterType, "Dispose");
		}

		public MySqlClientSqlCommandSet(int batchSize)
		{
			instance = Activator.CreateInstance(adapterType, true);
			doInitialise(instance);
			batchSizeSetter(instance, batchSize);
		}

		public void Append(DbCommand command)
		{
			doAppend(instance, command);
			countOfCommands++;
		}

		public void Dispose()
		{
			doDispose(instance);
		}

		public int ExecuteNonQuery()
		{
			try
			{
				if (CountOfCommands == 0)
				{
					return 0;
				}

				return doExecuteNonQuery(instance);
			}
			catch (Exception exception)
			{
				throw new HibernateException("An exception occured when executing batch queries", exception);
			}
		}

		public int CountOfCommands
		{
			get { return countOfCommands; }
		}
	}
}