using System;
using NHibernate.AdoNet;
using NHibernate.Impl;

namespace NHibernate.Transaction
{
	/// <summary>
	/// This is used as a marker interface for the different 
	/// transaction context required for each session
	/// </summary>
	public interface ITransactionContext : IDisposable
	{
		/// <summary>
		/// Is the transaction still active?
		/// </summary>
		bool IsInActiveTransaction { get; }
		/// <summary>
		/// Should the session be closed upon transaction completion?
		/// </summary>
		bool ShouldCloseSessionOnSystemTransactionCompleted { get; set; }
		/// <summary>
		/// Can the transaction completion trigger a flush?
		/// </summary>
		bool CanFlushOnSystemTransactionCompleted { get; }
		/// <summary>
		/// With some transaction factory, synchronization of session may be required. This method should be called
		/// by session before each of its usage where a concurrent transaction completion action could cause a thread
		/// safety issue. This method is already called by <see cref="AbstractSessionImpl.CheckAndUpdateSessionStatus"/>
		/// and <see cref="AbstractSessionImpl.BeginProcess"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is required due to MSDTC asynchronism. When a transaction is promoted to distributed, MSDTC
		/// starts handling it. See https://github.com/npgsql/npgsql/issues/1571#issuecomment-308651461 for a discussion
		/// about it.
		/// </para>
		/// <para>
		/// MSDTC considers the transaction to be committed as soon as it has collected all positive votes from prepare
		/// phases of enlisted resources
		/// (<see cref="System.Transactions.IEnlistmentNotification.Prepare(System.Transactions.PreparingEnlistment)"/>).
		/// It then concurrently lets the <see cref="System.Transactions.TransactionScope"/> disposal leave and allow
		/// the code following it to execute, raises transaction completion event
		/// (<see cref="System.Transactions.Transaction.TransactionCompleted" />) and calls all resources second phase
		/// callbacks (<see cref="System.Transactions.IEnlistmentNotification.Commit(System.Transactions.Enlistment)"/>).
		/// </para>
		/// <para>
		/// For rollback cases, it depends on what has triggered the rollback. The transaction is marked as aborted. The
		/// transaction completion event is raised. If the rollback has been triggered by a resource prepare phase, the
		/// rollback callback of that resource will not be called. Prepare phase may not have been called at all for some
		/// rollback cases. The called rollback callbacks execute concurrently with transaction completion event and
		/// code following the scope disposal.
		/// (See (<see cref="System.Transactions.IEnlistmentNotification.Rollback(System.Transactions.Enlistment)"/>.)
		/// </para>
		/// <para>
		/// In-doubt cases are similar to rollback cases. The transaction completion event is raised too, and run
		/// concurrently to in-doubt callbacks
		/// (<see cref="System.Transactions.IEnlistmentNotification.InDoubt(System.Transactions.Enlistment)"/>) and
		/// code following the scope disposal.
		/// </para>
		/// <para>
		/// Due to this, for avoiding concurrency races, this method should block before the last resource signals it is
		/// prepared (<see cref="System.Transactions.PreparingEnlistment.Prepared"/>), and if it detects the transaction
		/// is no more active (<see cref="System.Transactions.TransactionStatus.Active"/>) while not having already
		/// blocked. It should be released only once the <see cref="ISession"/> and <see cref="ConnectionManager"/>
		/// transaction completion events and cleanups have been handled.
		/// </para>
		/// </remarks>
		void Wait();
	}
}
