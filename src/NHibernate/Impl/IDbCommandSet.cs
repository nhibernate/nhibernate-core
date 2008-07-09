using System;
using System.Data;

namespace NHibernate.Impl
{
    internal interface IDbCommandSet : IDisposable
    {
        /// <summary>
        /// Append a command to the batch
        /// </summary>
        /// <param name="command"></param>
        void Append(IDbCommand command);

        /// <summary>
        /// Return the batch command to be executed
        /// </summary>
        IDbCommand BatchCommand { get; }

        /// <summary>
        /// The number of commands batched in this instance
        /// </summary>
        int CountOfCommands { get; }

        /// <summary>
        /// Executes the batch
        /// </summary>
        /// <returns>
        /// This seems to be returning the total number of affected rows in all queries
        /// </returns>
        int ExecuteNonQuery();
    }
}
