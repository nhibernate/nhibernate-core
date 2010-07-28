using System;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Engine
{
	/// <summary>
	/// Manages <see cref="IDbCommand"/>s and <see cref="IDataReader"/>s 
	/// for an <see cref="ISession"/>. 
	/// </summary>
	/// <remarks>
	/// <p>
	/// Abstracts ADO.NET batching to maintain the illusion that a single logical batch 
	/// exists for the whole session, even when batching is disabled.
	/// Provides transparent <c>IDbCommand</c> caching.
	/// </p>
	/// <p>
	/// This will be useful once ADO.NET gets support for batching.  Until that point
	/// no code exists that will do batching, but this will provide a good point to do
	/// error checking and making sure the correct number of rows were affected.
	/// </p>
	/// </remarks>
	public interface IBatcher : IDisposable
	{
		/// <summary>
		/// Get an <see cref="IDbCommand"/> for using in loading / querying.
		/// </summary>
		/// <param name="sql">The <see cref="SqlString"/> to convert to an <see cref="IDbCommand"/>.</param>
		/// <param name="commandType">The <see cref="CommandType"/> of the command.</param>
		/// <param name="parameterTypes">The <see cref="SqlType">SqlTypes</see> of parameters
		/// in <paramref name="sql" />.</param>
		/// <returns>
		/// An <see cref="IDbCommand"/> that is ready to be executed.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If not explicitly released by <see cref="CloseCommand" />, it will be 
		/// released when the session is closed or disconnected.
		/// </para>
		/// <para>
		/// This does NOT add anything to the batch - it only creates the IDbCommand and 
		/// does NOT cause the batch to execute...
		/// </para>
		/// </remarks>
		IDbCommand PrepareQueryCommand(CommandType commandType, SqlString sql, SqlType[] parameterTypes);

		/// <summary>
		/// Get a non-batchable an <see cref="IDbCommand"/> to use for inserting / deleting / updating.
		/// Must be explicitly released by <c>CloseCommand()</c>
		/// </summary>
		/// <param name="sql">The <see cref="SqlString"/> to convert to an <see cref="IDbCommand"/>.</param>
		/// <param name="commandType">The <see cref="CommandType"/> of the command.</param>
		/// <param name="parameterTypes">The <see cref="SqlType">SqlTypes</see> of parameters
		/// in <paramref name="sql" />.</param>
		/// <returns>
		/// An <see cref="IDbCommand"/> that is ready to have the parameter values set
		/// and then executed.
		/// </returns>
		IDbCommand PrepareCommand(CommandType commandType, SqlString sql, SqlType[] parameterTypes);

		/// <summary>
		/// Close a <see cref="IDbCommand"/> opened using <c>PrepareCommand()</c>
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to ensure is closed.</param>
		/// <param name="reader">The <see cref="IDataReader"/> to ensure is closed.</param>
		void CloseCommand(IDbCommand cmd, IDataReader reader);

		/// <summary>
		/// Close a <see cref="IDataReader"/> opened using <see cref="ExecuteReader"/>
		/// </summary>
		/// <param name="reader">The <see cref="IDataReader"/> to ensure is closed.</param>
		void CloseReader(IDataReader reader);

		/// <summary>
		/// Get a batchable <see cref="IDbCommand"/> to use for inserting / deleting / updating
		/// (might be called many times before a single call to <c>ExecuteBatch()</c>
		/// </summary>
		/// <remarks>
		/// After setting parameters, call <c>AddToBatch()</c> - do not execute the statement
		/// explicitly.
		/// </remarks>
		/// <param name="sql">The <see cref="SqlString"/> to convert to an <see cref="IDbCommand"/>.</param>
		/// <param name="commandType">The <see cref="CommandType"/> of the command.</param>
		/// <param name="parameterTypes">The <see cref="SqlType">SqlTypes</see> of parameters
		/// in <paramref name="sql" />.</param>
		/// <returns></returns>
		IDbCommand PrepareBatchCommand(CommandType commandType, SqlString sql, SqlType[] parameterTypes);

		/// <summary>
		/// Add an insert / delete / update to the current batch (might be called multiple times
		/// for a single <c>PrepareBatchStatement()</c>)
		/// </summary>
		/// <param name="expectation">Determines whether the number of rows affected by query is correct.</param>
		void AddToBatch(IExpectation expectation);

		/// <summary>
		/// Execute the batch
		/// </summary>
		void ExecuteBatch();

		/// <summary>
		/// Close any query statements that were left lying around
		/// </summary>
		/// <remarks>
		/// Use this method instead of <c>Dispose</c> if the <see cref="IBatcher"/>
		/// can be used again.
		/// </remarks>
		void CloseCommands();

		/// <summary>
		/// Gets an <see cref="IDataReader"/> by calling ExecuteReader on the <see cref="IDbCommand"/>.
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to execute to get the <see cref="IDataReader"/>.</param>
		/// <returns>The <see cref="IDataReader"/> from the <see cref="IDbCommand"/>.</returns>
		/// <remarks>
		/// The Batcher is responsible for ensuring that all of the Drivers rules for how many open
		/// <see cref="IDataReader"/>s it can have are followed.
		/// </remarks>
		IDataReader ExecuteReader(IDbCommand cmd);

		/// <summary>
		/// Executes the <see cref="IDbCommand"/>. 
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to execute.</param>
		/// <returns>The number of rows affected.</returns>
		/// <remarks>
		/// The Batcher is responsible for ensuring that all of the Drivers rules for how many open
		/// <see cref="IDataReader"/>s it can have are followed.
		/// </remarks>
		int ExecuteNonQuery(IDbCommand cmd);

		/// <summary>
		/// Expand the parameters of the cmd to have a single parameter for each parameter in the
		/// sql string
		/// </summary>
		/// <remarks>
		/// This is for databases that do not support named parameters.  So, instead of a single parameter
		/// for 'select ... from MyTable t where t.Col1 = @p0 and t.Col2 = @p0' we can issue
		/// 'select ... from MyTable t where t.Col1 = ? and t.Col2 = ?'
		/// </remarks>
		void ExpandQueryParameters(IDbCommand cmd, SqlString sqlString);

		/// <summary>
		/// Must be called when an exception occurs.
		/// </summary>
		/// <param name="e"></param>
		void AbortBatch(Exception e);

		/// <summary>
		/// Cancel the current query statement
		/// </summary>
		void CancelLastQuery();

		/// <summary>
		/// Gets the value indicating whether there are any open resources
		/// managed by this batcher (IDbCommands or IDataReaders).
		/// </summary>
		bool HasOpenResources { get; }

		/// <summary>
		/// Gets or sets the size of the batch, this can change dynamically by
		/// calling the session's SetBatchSize.
		/// </summary>
		/// <value>The size of the batch.</value>
		int BatchSize { get; set; }
	}
}