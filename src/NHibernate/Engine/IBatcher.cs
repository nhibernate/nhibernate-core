using System;
using System.Data;
using NHibernate.SqlCommand;

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
		/// <param name="type">The <see cref="CommandType"/> of the command.</param>
		/// <returns>
		/// An <see cref="IDbCommand"/> that is ready to be executed.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If not explicitly released by <c>CloseQueryStatement()</c>, it will be 
		/// released when the session is closed or disconnected.
		/// </para>
		/// <para>
		/// This does NOT add anything to the batch - it only creates the IDbCommand and 
		/// does NOT cause the batch to execute...
		/// </para>
		/// </remarks>
		IDbCommand PrepareQueryCommand( SqlString sql, CommandType commandType );

		/// <summary>
		/// Closes the <see cref="IDbCommand"/> &amp; the <see cref="IDataReader"/> that was
		/// opened with the method <c>PrepareQueryCommand</c>.
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to close.</param>
		/// <param name="reader">The <see cref="IDataReader"/> to close.</param>
		void CloseQueryCommand( IDbCommand cmd, IDataReader reader );

		/// <summary>
		/// Get a non-batchable an <see cref="IDbCommand"/> to use for inserting / deleting / updating.
		/// Must be explicitly released by <c>CloseCommand()</c>
		/// </summary>
		/// <param name="sql">The <see cref="SqlString"/> to convert to an <see cref="IDbCommand"/>.</param>
		/// <param name="type">The <see cref="CommandType"/> of the command.</param>
		/// <returns>
		/// An <see cref="IDbCommand"/> that is ready to have the parameter values set
		/// and then executed.
		/// </returns>
		IDbCommand PrepareCommand( SqlString sql, CommandType type );

		/// <summary>
		/// Close a <see cref="IDbCommand"/> opened using <c>PrepareCommand()</c>
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to ensure is closed.</param>
		/// <param name="reader">The <see cref="IDataReader"/> to ensure is closed.</param>
		void CloseCommand( IDbCommand cmd, IDataReader reader );

		/// <summary>
		/// Get a batchable <see cref="IDbCommand"/> to use for inserting / deleting / updating
		/// (might be called many times before a single call to <c>ExecuteBatch()</c>
		/// </summary>
		/// <remarks>
		/// After setting parameters, call <c>AddToBatch()</c> - do not execute the statement
		/// explicitly.
		/// </remarks>
		/// <param name="sql">The <see cref="SqlString"/> to convert to an <see cref="IDbCommand"/>.</param>
		/// <param name="type">The <see cref="CommandType"/> of the command.</param>
		/// <returns></returns>
		IDbCommand PrepareBatchCommand( SqlString sql, CommandType type );

		/// <summary>
		/// Add an insert / delete / update to the current batch (might be called multiple times
		/// for a single <c>PrepareBatchStatement()</c>)
		/// </summary>
		/// <param name="expectedRowCount">The number of rows that should be affected when the query is run.</param>
		/// <remarks>
		/// A negative number in expectedRowCount means that you don't know how many rows to 
		/// expect.
		/// </remarks>
		void AddToBatch( int expectedRowCount );

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
		IDataReader ExecuteReader( IDbCommand cmd );

		/// <summary>
		/// Executes the <see cref="IDbCommand"/>. 
		/// </summary>
		/// <param name="cmd">The <see cref="IDbCommand"/> to execute.</param>
		/// <returns>The number of rows affected.</returns>
		/// <remarks>
		/// The Batcher is responsible for ensuring that all of the Drivers rules for how many open
		/// <see cref="IDataReader"/>s it can have are followed.
		/// </remarks>
		int ExecuteNonQuery( IDbCommand cmd );

		/// <summary>
		/// Must be called when an exception occurs.
		/// </summary>
		/// <param name="e"></param>
		void AbortBatch( Exception e );

		/// <summary>
		/// Cancel the current query statement
		/// </summary>
		void CancelLastQuery();

		/// <summary>
		/// Generates an <see cref="IDbCommand"/> from a <see cref="SqlString"/>.
		/// </summary>
		/// <param name="sqlString">The <see cref="SqlString"/> to use to generate an <see cref="IDbCommand"/>.</param>
		/// <returns>An <see cref="IDbCommand"/> that is not attached to a Connection or Transaction.</returns>
		/// <remarks>
		/// A wrapper for calling the <c>IDriver.GenerateCommand</c> that adds logging.
		/// </remarks>
		IDbCommand Generate( SqlString sqlString, CommandType type );

	}
}