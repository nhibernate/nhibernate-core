using System;
using System.Data;

namespace NHibernate.Engine {

	/// <summary>
	/// Manages <c>IDbCommand</c>s for a session. 
	/// </summary>
	/// <remarks>
	/// Abstracts ADO.NET batching to maintain the illusion
	/// that a single logical batch exists for the whole session, even when batching is disabled.
	/// Provides transparent <c>IDbCommand</c> caching.
	/// 
	/// TODO: DESIGNQUESTION: we might want to use this to tie together the Connection.IConnection and Transaction.ITransaction because
	/// of how closely ADO.NET ties together the IDbConnection and IDbTransaction and IDbCommand - namely that creating
	/// a IDbCommand from an IDbConnection doesn't automattically give it the IDbTransaction.
	/// </remarks>
	public interface IBatcher {
		
		/// <summary>
		/// Get a prepared statement for using in loading / querying.
		/// </summary>
		/// <remarks>
		/// If not explicitly released by <c>CloseQueryStatement()</c>, it will be 
		/// released when the session is closed or disconnected.
		/// 
		/// This does NOT add anything to the batch - it only creates the IDbCommand and 
		/// does NOT cause the batch to execute...
		/// </remarks>
		IDbCommand PrepareQueryStatement(string sql);

		/// <summary>
		/// Closes a command opened with <c>PrepareQueryStatement</c>
		/// </summary>
		/// <param name="db"></param>
		void CloseQueryStatement(IDbCommand cm);

		/// <summary>
		/// Get a non-batchable prepared statement to use for inserting / deleting / updating.
		/// Must be explicitly released by <c>CloseStatement()</c>
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		IDbCommand PrepareStatement(string sql);

		/// <summary>
		/// Close a prepared statement opened using <c>PrepareStatement()</c>
		/// </summary>
		/// <param name="cm"></param>
		void CloseStatement(IDbCommand cm);

		/// <summary>
		/// Get a batchable prepared statement to use for inserting / deleting / updating
		/// (might be called many times before a single call to <c>ExecuteBatch()</c>
		/// </summary>
		/// <remarks>
		/// After setting parameters, call <c>AddToBatch()</c> - do not execute the statement
		/// explicitly
		/// </remarks>
		/// <param name="sql"></param>
		/// <returns></returns>
		IDbCommand PrepareBatchStatement(string sql);

		/// <summary>
		/// Add an insert / delete / update to the current batch (might be called multiple times
		/// for a single <c>PrepareBatchStatement()</c>)
		/// </summary>
		/// <param name="expectedRowCount"></param>
		void AddToBatch(int expectedRowCount);

		/// <summary>
		/// Execute the batch
		/// </summary>
		void ExecuteBatch();

		/// <summary>
		/// Close any query statements that were left lying around
		/// </summary>
		void CloseStatements();
	}
}
