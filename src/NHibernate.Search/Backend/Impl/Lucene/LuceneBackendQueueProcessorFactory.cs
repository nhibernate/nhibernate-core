using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif
using System.Threading;

namespace NHibernate.Search.Backend.Impl.Lucene
{
    public class LuceneBackendQueueProcessorFactory : IBackendQueueProcessorFactory
    {
        private SearchFactory searchFactory;

        public void Initialize(IDictionary props, SearchFactory searchFactory)
        {
            this.searchFactory = searchFactory;
        }

#if NET_2_0
        public WaitCallback GetProcessor(List<LuceneWork> queue)
        {
            return new LuceneBackendQueueProcessor(queue, searchFactory).Run;
        }
#else
		public WaitCallback GetProcessor(IList queue)
		{
			LuceneBackendQueueProcessor proc = new LuceneBackendQueueProcessor(queue, searchFactory);
			return new WaitCallback(proc.Run);
		}
#endif
    }
}