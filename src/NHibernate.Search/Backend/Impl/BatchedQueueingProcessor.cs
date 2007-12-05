using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Search.Backend.Impl.Lucene;
using NHibernate.Search.Engine;
using NHibernate.Search.Impl;
using NHibernate.Util;

namespace NHibernate.Search.Backend.Impl
{
    /// <summary>
    ///  Batch work until #ExecuteQueue is called.
    ///  The work is then executed synchronously or asynchronously
    /// </summary>
    public class BatchedQueueingProcessor : IQueueingProcessor
    {
        private readonly IBackendQueueProcessorFactory backendQueueProcessorFactory;
        private readonly bool sync;
        private SearchFactory searchFactory;

        public BatchedQueueingProcessor(SearchFactory searchFactory,
                                        IDictionary properties)
        {
            this.searchFactory = searchFactory;
            //default to sync if none defined
            sync =
                !"async".Equals((string) properties[Environment.WorkerExecution],
                                StringComparison.InvariantCultureIgnoreCase);

            string backend = (string) properties[Environment.WorkerBackend];
            if (StringHelper.IsEmpty(backend) || "lucene".Equals(backend, StringComparison.InvariantCultureIgnoreCase))
            {
                backendQueueProcessorFactory = new LuceneBackendQueueProcessorFactory();
            }
            else
            {
                try
                {
                    System.Type processorFactoryClass = ReflectHelper.ClassForName(backend);
                    backendQueueProcessorFactory =
                        (IBackendQueueProcessorFactory) Activator.CreateInstance(processorFactoryClass);
                }
                catch (Exception e)
                {
                    throw new SearchException("Unable to find/create processor class: " + backend, e);
                }
            }
            backendQueueProcessorFactory.Initialize(properties, searchFactory);
            searchFactory.SetbackendQueueProcessorFactory(backendQueueProcessorFactory);
        }


        //TODO implements parallel batchWorkers (one per Directory)
        public void PerformWork(List<LuceneWork> luceneQueue)
        {
            WaitCallback processor = backendQueueProcessorFactory.GetProcessor(luceneQueue);
            if (sync)
            {
                processor(null);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(processor);
            }
        }

        public void CancelWork(List<LuceneWork> queue)
        {
            queue.Clear();
        }
    }
}