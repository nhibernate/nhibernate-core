using System;
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
		/// safety issue. This method is already called by <see cref="AbstractSessionImpl.CheckAndUpdateSessionStatus"/>.
		/// </summary>
		void Wait();
	}
}