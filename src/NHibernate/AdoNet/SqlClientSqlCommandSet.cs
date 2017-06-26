using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.AdoNet
{
	using SqlCommand = System.Data.SqlClient.SqlCommand;

	/// <summary>
	/// Expose the batch functionality in ADO.Net 4.0
	/// Microsoft in its wisdom decided to make my life hard and mark it internal.
	/// Through the use of Reflection and some delegates magic, I opened up the functionality.
	/// 
	/// Observable performance benefits are 50%+ when used, so it is really worth it.
	/// </summary>
	public class SqlClientSqlCommandSet : IDisposable
	{
		private static readonly System.Type sqlCmdSetType;
		private static readonly Action<object, SqlConnection> connectionSetter;
		private static readonly Action<object, SqlTransaction> transactionSetter;
		private static readonly Action<object, int> commandTimeoutSetter;
		private static readonly Func<object, SqlConnection> connectionGetter;
		private static readonly Func<object, SqlCommand> batchCommandGetter;
		private static readonly Action<object, SqlCommand> doAppend;
		private static readonly Func<object, int> doExecuteNonQuery;
		private static readonly Action<object> doDispose;

		private readonly object instance;
		private int countOfCommands;

		static SqlClientSqlCommandSet()
		{
			var sysData = Assembly.Load("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
			Debug.Assert(sqlCmdSetType != null, "Could not find SqlCommandSet!");

			connectionSetter = DelegateHelper.BuildPropertySetter<SqlConnection>(sqlCmdSetType, "Connection");
			connectionGetter = DelegateHelper.BuildPropertyGetter<SqlConnection>(sqlCmdSetType, "Connection");
			transactionSetter = DelegateHelper.BuildPropertySetter<SqlTransaction>(sqlCmdSetType, "Transaction");
			commandTimeoutSetter = DelegateHelper.BuildPropertySetter<int>(sqlCmdSetType, "CommandTimeout");
			batchCommandGetter = DelegateHelper.BuildPropertyGetter<SqlCommand>(sqlCmdSetType, "BatchCommand");
			doAppend = DelegateHelper.BuildAction<SqlCommand>(sqlCmdSetType, "Append");
			doExecuteNonQuery = DelegateHelper.BuildFunc<int>(sqlCmdSetType, "ExecuteNonQuery");
			doDispose = DelegateHelper.BuildAction(sqlCmdSetType, "Dispose");
		}

		public SqlClientSqlCommandSet()
		{
			instance = Activator.CreateInstance(sqlCmdSetType, true);
		}

		/// <summary>
		/// Append a command to the batch
		/// </summary>
		/// <param name="command"></param>
		public void Append(SqlCommand command)
		{
			AssertHasParameters(command);
			doAppend(instance, command);
			countOfCommands++;
		}

		/// <summary>
		/// This is required because SqlClient.SqlCommandSet will throw if 
		/// the command has no parameters.
		/// </summary>
		/// <param name="command"></param>
		private static void AssertHasParameters(SqlCommand command)
		{
			if (command.Parameters.Count == 0)
			{
				throw new ArgumentException("A command in SqlCommandSet must have parameters. You can't pass hardcoded sql strings.");
			}
		}


		/// <summary>
		/// Return the batch command to be executed
		/// </summary>
		public SqlCommand BatchCommand
		{
			get { return batchCommandGetter(instance); }
		}

		/// <summary>
		/// The number of commands batched in this instance
		/// </summary>
		public int CountOfCommands
		{
			get { return countOfCommands; }
		}

		/// <summary>
		/// Executes the batch
		/// </summary>
		/// <returns>
		/// This seems to be returning the total number of affected rows in all queries
		/// </returns>
		public int ExecuteNonQuery()
		{
			if (Connection == null)
				throw new ArgumentNullException(
					"Connection was not set! You must set the connection property before calling ExecuteNonQuery()");

			if (CountOfCommands == 0)
				return 0;
			return doExecuteNonQuery(instance);
		}

		public SqlConnection Connection
		{
			get { return connectionGetter(instance); }
			set { connectionSetter(instance, value); }
		}

		public SqlTransaction Transaction
		{
			set { transactionSetter(instance, value); }
		}

		public int CommandTimeout
		{
			set { commandTimeoutSetter(instance, value); }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			doDispose(instance);
		}
	}
}
