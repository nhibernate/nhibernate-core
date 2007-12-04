using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif
using System.Threading;

namespace NHibernate.Search.Backend
{
    /// <summary>
    /// Build stateful backend processor
    /// Must have a no arg constructor
    /// The factory typically prepare or pool the resources needed by the queue processor
    /// </summary>
    public interface IBackendQueueProcessorFactory
    {
        void Initialize(IDictionary props, SearchFactory searchFactory);

#if NET_2_0
        /// <summary>
        /// Return a runnable implementation responsible for processing the queue to a given backend
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        WaitCallback GetProcessor(List<LuceneWork> queue);
#else
		/// <summary>
		/// Return a runnable implementation responsible for processing the queue to a given backend
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		WaitCallback GetProcessor(IList queue);
#endif
    }
}