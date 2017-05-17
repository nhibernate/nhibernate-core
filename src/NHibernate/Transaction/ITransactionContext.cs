using System;
using NHibernate.Engine;

namespace NHibernate.Transaction
{
	/// <summary>
	/// This is used as a marker interface for the different 
	/// transaction context required for each session
	/// </summary>
	public interface ITransactionContext : IDisposable
	{
		bool ShouldCloseSessionOnDistributedTransactionCompleted { get; set; }
		/// <summary>
		/// With some transaction factory, synchronization of session may be required. This method should be called
		/// by session before each of its usage where a concurrent transaction completion action could cause a thread
		/// safety issue. This method is already called by ITransactionFactory.EnlistInDistributedTransactionIfNeeded
		/// if the factory requires synchronization.
		/// </summary>
		void WaitOne();
	}
}