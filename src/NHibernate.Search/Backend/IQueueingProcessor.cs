#if NET_2_0
using System.Collections.Generic;
#else
using System.Collections;
#endif

namespace NHibernate.Search.Backend
{
    /// <summary>
    ///	 Pile work operations
    ///  No thread safety has to be implemented, the queue being scoped already
    ///  The implementation must be "stateless" wrt the queue through (ie not store the queue state)
    /// </summary>
    public interface IQueueingProcessor
    {
#if NET_2_0
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
#else
		/// <summary>
		/// Performs all the work in the queue
		/// </summary>
		/// <param name="queue">The queue.</param>
		void PerformWork(IList queue);

		/// <summary>
		/// Rollback 
		/// </summary>
		/// <param name="queue"></param>
		void CancelWork(IList queue);
#endif
    }
}