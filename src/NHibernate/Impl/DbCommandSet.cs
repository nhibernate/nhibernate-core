
using System;
using System.Data;

namespace NHibernate.Impl
{
    /// <summary>
    /// Expose the batch functionality in ADO.Net 2.0
    /// Microsoft in its wisdom decided to make my life hard and mark it internal.
    /// Through the use of Reflection and some delegates magic, I opened up the functionality.
    /// 
    /// Observable performance benefits are 50%+ when used, so it is really worth it.
    /// </summary>
    public abstract class DbCommandSet<TConnection, TCommand> : IDbCommandSet
		where TConnection : class, IDbConnection
		where TCommand : class, IDbCommand
    {
        private readonly PropGetter<TCommand> commandGetter;
        private readonly AppendCommand doAppend;
        private readonly ExecuteNonQueryCommand doExecuteNonQuery;
        private readonly DisposeCommand doDispose;
        private int countOfCommands;

        protected DbCommandSet()
        {
            object internalCommandSet = CreateInternalCommandSet();
            commandGetter =
                (PropGetter<TCommand>)
                Delegate.CreateDelegate(typeof(PropGetter<TCommand>), internalCommandSet,
                                        "get_BatchCommand");
            doAppend = (AppendCommand)Delegate.CreateDelegate(typeof(AppendCommand), internalCommandSet, "Append");
            doExecuteNonQuery = (ExecuteNonQueryCommand)
                                Delegate.CreateDelegate(typeof(ExecuteNonQueryCommand),
                                                        internalCommandSet, "ExecuteNonQuery");
            doDispose = (DisposeCommand)Delegate.CreateDelegate(typeof(DisposeCommand), internalCommandSet, "Dispose");
        }

        protected abstract object CreateInternalCommandSet();
    
        /// <summary>
        /// Append a command to the batch
        /// </summary>
        /// <param name="command"></param>
        public void Append(IDbCommand command)
        {
            AssertHasParameters(command);
            TCommand sqlCommand = (TCommand)command;
            doAppend(sqlCommand);
            countOfCommands++;
        }

        /// <summary>
        /// This is required because SqlClient.SqlCommandSet will throw if 
        /// the command has no parameters.
        /// </summary>
        /// <param name="command"></param>
        private static void AssertHasParameters(IDbCommand command)
        {
            if (command.Parameters.Count == 0)
            {
                throw new ArgumentException("A command in SqlCommandSet must have parameters. You can't pass hardcoded sql strings.");
            }
        }


        /// <summary>
        /// Return the batch command to be executed
        /// </summary>
        public IDbCommand BatchCommand
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
				throw new ArgumentNullException("Connection",
                    "Connection was not set! You must set the connection property before calling ExecuteNonQuery()");
            try
            {
                if (CountOfCommands == 0)
                    return 0;
                return doExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new HibernateException("An exception occured when executing batch queries", e);
            }
        }

        public TConnection Connection
        {
            get { return (TConnection) commandGetter().Connection; }
            set { commandGetter().Connection = value; }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            doDispose();
        }

        #region Delegate Definitions

        private delegate T PropGetter<T>();

        private delegate void AppendCommand(TCommand command);

        private delegate int ExecuteNonQueryCommand();

        private delegate void DisposeCommand();

        #endregion
    }
}

