using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace NHibernate.Search.Impl
{
	///  Apply the operations to Lucene directories
	///  avoiding deadlocks
	/// </summary>
	public class LuceneBackendQueueProcessor
	{
		private List<LuceneWork> queue;
		private SearchFactory searchFactory;

		public LuceneBackendQueueProcessor(List<LuceneWork> queue, SearchFactory searchFactory)
		{
			this.queue = queue;
			this.searchFactory = searchFactory;
		}

		public void Run(object ignored)
		{
			LuceneWorkspace luceneWorkspace = new LuceneWorkspace(searchFactory);
			LuceneWorker worker = new LuceneWorker(luceneWorkspace);
			try
			{
				SortQueueToAvoidDeadLocks(queue, luceneWorkspace);
				foreach (LuceneWork luceneWork in queue)
				{
					worker.PerformWork(luceneWork);
				}
			}
			finally
			{
				luceneWorkspace.Dispose();
				queue.Clear();
			}
		}

		/// <summary>
		/// one must lock the directory providers in the exact same order to avoid
		/// dead lock between concurrent threads or processes
		/// To achieve that, the work will be done per directory provider
		/// We rely on the both the DocumentBuilder.GetHashCode() and the GetWorkHashCode() to 
		/// sort them by predictive order at all times, and to put deletes before adds
		/// </summary>
		private void SortQueueToAvoidDeadLocks(List<LuceneWork> queue, LuceneWorkspace luceneWorkspace)
		{
			queue.Sort(delegate(LuceneWork x, LuceneWork y)
			{
				long h1 = GetWorkHashCode(x, luceneWorkspace);
				long h2 = GetWorkHashCode(y, luceneWorkspace);
				if (h1 < h2)
					return -1;
				else if (h1 == h2)
					return 0;
				else return 1;
			});
		}

		private long GetWorkHashCode(LuceneWork luceneWork, LuceneWorkspace luceneWorkspace)
		{
			long h = luceneWorkspace.GetDocumentBuilder(luceneWork.EntityClass).GetHashCode()*2;
			if (luceneWork is AddLuceneWork)
				h += 1; //addwork after deleteWork
			return h;
		}
	}
}