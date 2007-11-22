using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Search.Impl;
using NHibernate.Search.Storage;
using NHibernate.Util;

namespace NHibernate.Search
{
    public class SearchFactory
    {
        private static WeakHashtable sessionFactory2SearchFactory = new WeakHashtable();

        /// <summary>
        /// Note that we will lock on the values in this dictionary
        /// </summary>
        private Dictionary<IDirectoryProvider, object> lockableDirectoryProviders = new Dictionary<IDirectoryProvider, object>();

        private Dictionary<System.Type, DocumentBuilder> documentBuilders = new Dictionary<System.Type, DocumentBuilder>();
        private IBackendQueueProcessorFactory backendQueueProcessorFactory;
        private IQueueingProcessor queueingProcessor;
        private static object searchFactoryKey = new object();

        public Dictionary<System.Type, DocumentBuilder> DocumentBuilders
        {
            get { return documentBuilders; }
        }

        public static SearchFactory GetSearchFactory(ISession session)
        {
            SearchFactory searchFactory = (SearchFactory)sessionFactory2SearchFactory[session.SessionFactory];
            if (searchFactory == null)
            {
                throw new HibernateException(
                    @"A Full Text Query was attempted on a session whose factory is not initialized to use Full Text Querying. 
Did you forget to call SearchFactory.Initialize(sessionFactory) ? ");
            }
            return searchFactory;
        }

        public static SearchFactory GetSearchFactory(ISessionFactory sessionFactory)
        {
            return (SearchFactory)sessionFactory2SearchFactory[sessionFactory];
        }

        public static void Initialize(Configuration cfg, ISessionFactory sessionFactory)
        {
            //This is a bit tricky, but we basically need to have a way to attach 
            //a search factory to a session factory.
            //The session factory is keeping a reference to the items as long as it is alive, so we put
            //the search factory inside the cfg factory, and thus keeping the GC from collecting it.
            //We may want to find a better way
            SearchFactory searchFactory = new SearchFactory(cfg);
            sessionFactory.Items[searchFactoryKey] = searchFactory;
            sessionFactory2SearchFactory[sessionFactory] = searchFactory;
        }

        private SearchFactory(Configuration cfg)
        {
            System.Type analyzerClass;

            String analyzerClassName = cfg.GetProperty(Environment.AnalyzerClass);
            if (analyzerClassName != null)
            {
                try
                {
                    analyzerClass = ReflectHelper.ClassForName(analyzerClassName);
                }
                catch (Exception e)
                {
                    throw new SearchException(
                        string.Format("Lucene analyzer class '{0}' defined in property '{1}' could not be found.", analyzerClassName, Environment.AnalyzerClass),
                        e
                        );
                }
            }
            else
            {
                analyzerClass = typeof(StandardAnalyzer);
            }
            // Initialize analyzer
            Analyzer analyzer;
            try
            {
                analyzer = (Analyzer)Activator.CreateInstance(analyzerClass);
            }
            catch (InvalidCastException e)
            {
                throw new SearchException(
                    string.Format("Lucene analyzer does not implement {0}: {1}", typeof(Analyzer).FullName, analyzerClassName)
                    );
            }
            catch (Exception e)
            {
                throw new SearchException("Failed to instantiate lucene analyzer with type " + analyzerClassName);
            }
            this.queueingProcessor = new BatchedQueueingProcessor(this, cfg.Properties);

            DirectoryProviderFactory factory = new DirectoryProviderFactory();

            foreach (PersistentClass clazz in cfg.ClassMappings)
            {
                System.Type mappedClass = clazz.MappedClass;
                if (mappedClass != null && AttributeUtil.IsIndexed(mappedClass))
                {
                    IDirectoryProvider provider = factory.CreateDirectoryProvider(mappedClass, cfg, this);

                    DocumentBuilder documentBuilder = new DocumentBuilder(mappedClass, analyzer, provider);

                    documentBuilders.Add(mappedClass, documentBuilder);
                }
            }
            ISet<System.Type> classes = new HashedSet<System.Type>(documentBuilders.Keys);
            foreach (DocumentBuilder documentBuilder in documentBuilders.Values)
            {
                documentBuilder.PostInitialize(classes);
            }
        }

        public void ExecuteQueue(List<LuceneWork> luceneWork, ISession session)
        {
            if (session.Transaction.IsActive)
            {
                ISessionImplementor si = (ISessionImplementor)session;
                ((SearchInterceptor)si.Interceptor).RegisterSyncronization(si.Transaction, luceneWork);
            }
            else
            {
                ExecuteQueueImmediate(luceneWork);
            }
        }

        public void ExecuteQueueImmediate(List<LuceneWork> luceneWork)
        {
            queueingProcessor.PerformWork(luceneWork);
        }


        public DocumentBuilder GetDocumentBuilder(object entity)
        {
            System.Type type = NHibernateUtil.GetClass(entity);
            return GetDocumentBuilder(type);
        }

        public DocumentBuilder GetDocumentBuilder(System.Type type)
        {
            DocumentBuilder builder;
            DocumentBuilders.TryGetValue(type, out builder);
            return builder;
        }

        public IDirectoryProvider GetDirectoryProvider(System.Type entity)
        {
            return GetDocumentBuilder(entity).DirectoryProvider;
        }

        public object GetLockObjForDirectoryProvider(IDirectoryProvider provider)
        {
            return lockableDirectoryProviders[provider];
        }

        public void PerformWork(object entity, object id, ISession session, WorkType workType)
        {
            DocumentBuilder documentBuilder = GetDocumentBuilder(entity);
            if (documentBuilder == null)
                return;
            List<LuceneWork> queue = new List<LuceneWork>();
            documentBuilder.AddToWorkQueue(entity, id, workType, queue, this);
            ExecuteQueue(queue, session);
        }

        public void SetbackendQueueProcessorFactory(IBackendQueueProcessorFactory backendQueueProcessorFactory)
        {
            this.backendQueueProcessorFactory = backendQueueProcessorFactory;
        }

        public void RegisterDirectoryProviderForLocks(IDirectoryProvider provider)
        {
            if (lockableDirectoryProviders.ContainsKey(provider) == false)
            {
                lockableDirectoryProviders.Add(provider, new object());
            }
        }
    }
}