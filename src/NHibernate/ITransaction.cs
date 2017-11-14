using System;
using System.Data;
using System.Data.Common;
using NHibernate.Transaction;

namespace NHibernate
{
	/// <summary>
	/// Allows the application to define units of work, while maintaining abstraction from the
	/// underlying transaction implementation
	/// </summary>
	/// <remarks>
	/// A transaction is associated with a <c>ISession</c> and is usually instantiated by a call to
	/// <c>ISession.BeginTransaction()</c>. A single session might span multiple transactions since 
	/// the notion of a session (a conversation between the application and the datastore) is of
	/// coarser granularity than the notion of a transaction. However, it is intended that there be
	/// at most one uncommitted <c>ITransaction</c> associated with a particular <c>ISession</c>
	/// at a time. Implementors are not intended to be threadsafe.
	/// </remarks>
	public partial interface ITransaction : IDisposable
	{
		/// <summary>
		/// Begin the transaction with the default isolation level.
		/// </summary>
		void Begin();

		/// <summary>
		/// Begin the transaction with the specified isolation level.
		/// </summary>
		/// <param name="isolationLevel">Isolation level of the transaction</param>
		void Begin(IsolationLevel isolationLevel);

		/// <summary>
		/// Flush the associated <c>ISession</c> and end the unit of work.
		/// </summary>
		/// <remarks>
		/// This method will commit the underlying transaction if and only if the transaction
		/// was initiated by this object.
		/// </remarks>
		void Commit();

		/// <summary>
		/// Force the underlying transaction to roll back.
		/// </summary>
		void Rollback();

		/// <summary>
		/// Is the transaction in progress
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Was the transaction rolled back or set to rollback only?
		/// </summary>
		bool WasRolledBack { get; }

		/// <summary>
		/// Was the transaction successfully committed?
		/// </summary>
		/// <remarks>
		/// This method could return <see langword="false" /> even after successful invocation of <c>Commit()</c>
		/// </remarks>
		bool WasCommitted { get; }

		/// <summary>
		/// Enlist the <see cref="DbCommand"/> in the current Transaction.
		/// </summary>
		/// <param name="command">The <see cref="DbCommand"/> to enlist.</param>
		/// <remarks>
		/// It is okay for this to be a no op implementation.
		/// </remarks>
		void Enlist(DbCommand command);
	 

		/// <summary>
		/// Register a user synchronization callback for this transaction.
		/// </summary>
		/// <param name="synchronization">The <see cref="ISynchronization"/> callback to register.</param>
		void RegisterSynchronization(ISynchronization synchronization);
	}
}
