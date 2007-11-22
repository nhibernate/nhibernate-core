using System;
using System.Collections.Generic;

namespace NHibernate.Search.Impl
{
	/// <summary>
	///	 Pile work operations
	///  No thread safety has to be implemented, the queue being scoped already
	///  The implementation must be "stateless" wrt the queue through (ie not store the queue state)
	/// </summary>
	public interface IQueueingProcessor
	{
		/// <summary>
		/// Performs all the work in the queue
		/// </summary>
		/// <param name="queue">The queue.</param>
		void PerformWork(List<LuceneWork> queue);

		/// <summary>
		/// Rollback 
		/// </summary>
		/// <param name="queue"></param>
		void CancelWork(List<LuceneWork> queue);
	}
}