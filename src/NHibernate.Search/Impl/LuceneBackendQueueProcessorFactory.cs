using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace NHibernate.Search.Impl
{
	public class LuceneBackendQueueProcessorFactory : IBackendQueueProcessorFactory {
		private SearchFactory searchFactory;

		public void Initialize(IDictionary props, SearchFactory searchFactory) {
			this.searchFactory = searchFactory;
		}

		public WaitCallback GetProcessor(List<LuceneWork> queue) {
			return new LuceneBackendQueueProcessor( queue, searchFactory ).Run;
		}
	}

}