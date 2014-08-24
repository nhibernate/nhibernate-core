using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace NHibernate.AdoNet
{
	using Action = System.Action;
	using SqlCommand = System.Data.SqlClient.SqlCommand;

	/// <summary>
	/// Expose the batch functionality in ADO.Net 2.0
	/// Microsoft in its wisdom decided to make my life hard and mark it internal.
	/// Through the use of Reflection and some delegates magic, I opened up the functionality.
	/// 
	/// Observable performance benefits are 50%+ when used, so it is really worth it.
	/// </summary>
	public class SqlClientSqlCommandSet : IDisposable
	{
		private static readonly System.Type sqlCmdSetType;
		private readonly object instance;
		private readonly Action<SqlConnection> connectionSetter;
		private readonly Action<SqlTransaction> transactionSetter;
		private readonly Action<int> commandTimeoutSetter;
		private readonly Func<SqlConnection> connectionGetter;
		private readonly Func<SqlCommand> commandGetter;
		private readonly Action<SqlCommand> doAppend;
		private readonly Func<int> doExecuteNonQuery;
		private readonly Action doDispose;
		private int countOfCommands;

		static SqlClientSqlCommandSet()
		{
			var sysData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
			Debug.Assert(sqlCmdSetType != null, "Could not find SqlCommandSet!");
		}

		public SqlClientSqlCommandSet()
		{
			instance = Activator.CreateInstance(sqlCmdSetType, true);
			connectionSetter = (Action<SqlConnection>) Delegate.CreateDelegate(typeof (Action<SqlConnection>), instance, "set_Connection");
			transactionSetter = (Action<SqlTransaction>) Delegate.CreateDelegate(typeof (Action<SqlTransaction>), instance, "set_Transaction");
			commandTimeoutSetter = (Action<int>) Delegate.CreateDelegate(typeof (Action<int>), instance, "set_CommandTimeout");
			connectionGetter = (Func<SqlConnection>) Delegate.CreateDelegate(typeof (Func<SqlConnection>), instance, "get_Connection");
			commandGetter = (Func<SqlCommand>) Delegate.CreateDelegate(typeof (Func<SqlCommand>), instance, "get_BatchCommand");
			doAppend = (Action<SqlCommand>) Delegate.CreateDelegate(typeof (Action<SqlCommand>), instance, "Append");
			doExecuteNonQuery = (Func<int>) Delegate.CreateDelegate(typeof (Func<int>), instance, "ExecuteNonQuery");
			doDispose = (Action) Delegate.CreateDelegate(typeof (Action), instance, "Dispose");
		}

		/// <summary>
		/// Append a command to the batch
		/// </summary>
		/// <param name="command"></param>
		public void Append(SqlCommand command)
		{
			AssertHasParameters(command);
			doAppend(command);
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
			get { return commandGetter(); }
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
			return doExecuteNonQuery();
		}

		public SqlConnection Connection
		{
			get { return connectionGetter(); }
			set { connectionSetter(value); }
		}

		public SqlTransaction Transaction
		{
			set { transactionSetter(value); }
		}

		public int CommandTimeout
		{
			set { commandTimeoutSetter(value); }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			doDispose();
		}
	}
}
