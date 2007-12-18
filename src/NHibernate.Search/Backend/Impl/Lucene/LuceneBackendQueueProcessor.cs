using System;
using System.Collections.Generic;
using NHibernate.Search.Engine;
using NHibernate.Search.Impl;

namespace NHibernate.Search.Backend.Impl.Lucene
{
    /// <summary>
    /// Apply the operations to Lucene directories avoiding deadlocks
    /// </summary>
    public class LuceneBackendQueueProcessor
    {
		private readonly SearchFactory searchFactory;

        private readonly List<LuceneWork> queue;

        public LuceneBackendQueueProcessor(List<LuceneWork> queue, SearchFactory searchFactory)
        {
            this.queue = queue;
            this.searchFactory = searchFactory;
        }

        /// <summary>
        /// one must lock the directory providers in the exact same order to avoid
        /// dead lock between concurrent threads or processes
        /// To achieve that, the work will be done per directory provider
        /// We rely on the both the DocumentBuilder.GetHashCode() and the GetWorkHashCode() to 
        /// sort them by predictive order at all times, and to put deletes before adds
        /// </summary>
        private static void SortQueueToAvoidDeadLocks(List<LuceneWork> queue, Workspace luceneWorkspace)
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

        private static long GetWorkHashCode(LuceneWork luceneWork, Workspace luceneWorkspace)
        {
            long h = luceneWorkspace.GetDocumentBuilder(luceneWork.EntityClass).GetHashCode()*2;
            if (luceneWork is AddLuceneWork)
                h += 1; //addwork after deleteWork
            return h;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignore">Ignored, used to keep the delegate signature that WaitCallback requires</param>
        public void Run(object ignore)
        {
            Workspace workspace = new Workspace(searchFactory);
            LuceneWorker worker = new LuceneWorker(workspace);
            try
            {
                SortQueueToAvoidDeadLocks(queue, workspace);
                foreach (LuceneWork luceneWork in queue)
                {
                    worker.PerformWork(new LuceneWorker.WorkWithPayload(luceneWork, null));
                }
            }
            finally
            {
                workspace.Dispose();
                queue.Clear();
            }
        }
    }
}