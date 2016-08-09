using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;
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
		private static readonly Func<object> creator;
		private static readonly PropertyInfo connectionProperty;
		private static readonly PropertyInfo transactionProperty;
		private static readonly PropertyInfo commandTimeoutProperty;
		private static readonly PropertyInfo commandProperty;
		private static readonly MethodInfo appendMethod;
		private static readonly MethodInfo executeNonQueryMethod;
		private static readonly MethodInfo disposeMethod;
		private int countOfCommands;

		static SqlClientSqlCommandSet()
		{
			var sysData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
			Debug.Assert(sqlCmdSetType != null, "Could not find SqlCommandSet!");
			connectionProperty = sqlCmdSetType.GetProperty("Connection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
			transactionProperty = sqlCmdSetType.GetProperty("Transaction", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
			commandTimeoutProperty = sqlCmdSetType.GetProperty("CommandTimeout", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
			commandProperty = sqlCmdSetType.GetProperty("BatchCommand", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
			appendMethod = sqlCmdSetType.GetMethod("Append", BindingFlags.NonPublic | BindingFlags.Instance);
			executeNonQueryMethod = sqlCmdSetType.GetMethod("ExecuteNonQuery", BindingFlags.NonPublic | BindingFlags.Instance);
			disposeMethod = sqlCmdSetType.GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Instance);
			creator = Expression.Lambda(Expression.Convert(Expression.New(sqlCmdSetType),typeof(object))).Compile() as Func<object>;
		}

		public SqlClientSqlCommandSet()
		{
			instance = creator();
		}

		/// <summary>
		/// Append a command to the batch
		/// </summary>
		/// <param name="command"></param>
		public void Append(SqlCommand command)
		{
			AssertHasParameters(command);
			appendMethod.Invoke(instance, new object[] { command });
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
			get { return (SqlCommand)commandProperty.GetValue(instance, null); }
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
			return (int)executeNonQueryMethod.Invoke(instance, null);
		}

		public SqlConnection Connection
		{
			get { return (SqlConnection)connectionProperty.GetValue(instance, null); }
			set { connectionProperty.SetValue(instance, value, null); }
		}

		public SqlTransaction Transaction
		{
			set { transactionProperty.SetValue(instance, value, null); }
		}

		public int CommandTimeout
		{
			set { commandTimeoutProperty.SetValue(instance, value, null); }
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			disposeMethod.Invoke(instance, null);
		}
	}
}
