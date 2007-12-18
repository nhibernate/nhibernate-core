using System;
using System.Collections.Generic;
using System.Reflection;
using Iesi.Collections.Generic;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using NHibernate.Search.Attributes;
using NHibernate.Search.Backend;
using NHibernate.Search.Bridge;
using NHibernate.Search.Impl;
using NHibernate.Search.Storage;
using NHibernate.Util;
using FieldInfo=System.Reflection.FieldInfo;

namespace NHibernate.Search.Engine
{
    /// <summary>
    /// Set up and provide a manager for indexes classes
    /// </summary>
    public class DocumentBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentBuilder));

        private readonly PropertiesMetadata rootPropertiesMetadata = new PropertiesMetadata();
        private readonly System.Type beanClass;
        private readonly IDirectoryProvider directoryProvider;
        private String idKeywordName;
        private MemberInfo idGetter;
        private readonly Analyzer analyzer;
        private float? idBoost;
        public const string CLASS_FIELDNAME = "_hibernate_class";
        private ITwoWayFieldBridge idBridge;
        private ISet<System.Type> mappedSubclasses = new HashedSet<System.Type>();
        private int level = 0;
        private int maxLevel = int.MaxValue;


        public DocumentBuilder(System.Type clazz, Analyzer analyzer, IDirectoryProvider directory)
        {
            beanClass = clazz;
            this.analyzer = analyzer;
            directoryProvider = directory;

            if (clazz == null) throw new AssertionFailure("Unable to build a DocumemntBuilder with a null class");

            rootPropertiesMetadata.boost = GetBoost(clazz);
            Set<System.Type> processedClasses = new HashedSet<System.Type>();
            processedClasses.Add(clazz);
            InitializeMembers(clazz, rootPropertiesMetadata, true, "", processedClasses);
            //processedClasses.remove( clazz ); for the sake of completness

            if (idKeywordName == null)
            {
                throw new SearchException("No document id for: " + clazz.Name);
            }
        }

        private void InitializeMembers(
            System.Type clazz, PropertiesMetadata propertiesMetadata, bool isRoot, String prefix,
            ISet<System.Type> processedClasses)
        {
            PropertyInfo[] propertyInfos = clazz.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                InitializeMember(propertyInfo, propertiesMetadata, isRoot, prefix, processedClasses);
            }

            FieldInfo[] fields = clazz.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fieldInfo in fields)
            {
                InitializeMember(fieldInfo, propertiesMetadata, isRoot, prefix, processedClasses);
            }
        }

        private void InitializeMember(
            MemberInfo member, PropertiesMetadata propertiesMetadata, bool isRoot,
            String prefix, ISet<System.Type> processedClasses)
        {
            DocumentIdAttribute documentIdAnn = AttributeUtil.GetDocumentId(member);
            if (isRoot && documentIdAnn != null)
            {
                if (idKeywordName != null)
                {
                    if (documentIdAnn.Name != null)
                        throw new AssertionFailure("Two document id assigned: "
                                                   + idKeywordName + " and " + documentIdAnn.Name);
                }
                idKeywordName = prefix + documentIdAnn.Name;
                IFieldBridge fieldBridge = BridgeFactory.GuessType(member);
                if (fieldBridge is ITwoWayFieldBridge)
                {
                    idBridge = (ITwoWayFieldBridge) fieldBridge;
                }
                else
                {
                    throw new SearchException(
                        "Bridge for document id does not implement IdFieldBridge: " + member.Name);
                }
                idBoost = GetBoost(member);
                idGetter = member;
            }

            FieldAttribute fieldAnn = AttributeUtil.GetField(member)
                ;
            if (fieldAnn != null)
            {
                propertiesMetadata.fieldGetters.Add(member);
                propertiesMetadata.fieldNames.Add(prefix + fieldAnn.Name);
                propertiesMetadata.fieldStore.Add(GetStore(fieldAnn.Store));
                propertiesMetadata.fieldIndex.Add(GetIndex(fieldAnn.Index));
                propertiesMetadata.fieldBridges.Add(BridgeFactory.GuessType(member));
            }
        }


        private static Field.Store GetStore(Attributes.Store store)
        {
            switch (store)
            {
                case Attributes.Store.No:
                    return Field.Store.NO;
                case Attributes.Store.Yes:
                    return Field.Store.YES;
                case Attributes.Store.Compress:
                    return Field.Store.COMPRESS;
                default:
                    throw new AssertionFailure("Unexpected Store: " + store);
            }
        }

        private static Field.Index GetIndex(Index index)
        {
            switch (index)
            {
                case Index.No:
                    return Field.Index.NO;
                case Index.NoNormalization:
                    return Field.Index.NO_NORMS;
                case Index.Tokenized:
                    return Field.Index.TOKENIZED;
                case Index.UnTokenized:
                    return Field.Index.UN_TOKENIZED;
                default:
                    throw new AssertionFailure("Unexpected Index: " + index);
            }
        }

        private static float? GetBoost(MemberInfo element)
        {
            if (element == null) return null;
            BoostAttribute boost = AttributeUtil.GetBoost(element);
            if (boost == null)
                return null;
            return boost.Value;
        }

        private static object GetMemberValue(Object instnace, MemberInfo getter)
        {
            PropertyInfo info = getter as PropertyInfo;
            if (info != null)
                return info.GetValue(instnace, null);
            else
                return ((FieldInfo) getter).GetValue(instnace);
        }

        /// <summary>
        /// This add the new work to the queue, so it can be processed in a batch fashion later
        /// </summary>
        public void AddToWorkQueue(object entity, object id, WorkType workType, List<LuceneWork> queue,
                                   SearchFactory searchFactory)
        {
            System.Type entityClass = NHibernateUtil.GetClass(entity);
            foreach (LuceneWork luceneWork in queue)
            {
                if (luceneWork.EntityClass == entityClass && luceneWork.Id.Equals(id))
                    return;
            }
            /*bool searchForContainers = false;*/
            string idString = idBridge.ObjectToString(id);

            switch (workType)
            {
                case WorkType.Add:
                    queue.Add(new AddLuceneWork(id, idString, entityClass, GetDocument(entity, id)));
                    /*searchForContainers = true;*/
                    break;

                case WorkType.Delete:
                case WorkType.Purge:
                    queue.Add(new DeleteLuceneWork(id, idString, entityClass));
                    break;

                case WorkType.PurgeAll:
                    queue.Add(new PurgeAllLuceneWork(entityClass));
                    break;

                case WorkType.Update:
                    /**
                     * even with Lucene 2.1, use of indexWriter to update is not an option
                     * We can only delete by term, and the index doesn't have a term that
                     * uniquely identify the entry.
                     * But essentially the optimization we are doing is the same Lucene is doing, the only extra cost is the
                     * double file opening.
                    */
                    queue.Add(new DeleteLuceneWork(id, idString, entityClass));
                    queue.Add(new AddLuceneWork(id, idString, entityClass, GetDocument(entity, id)));
                    /*searchForContainers = true;*/
                    break;

                default:
                    throw new AssertionFailure("Unknown WorkType: " + workType);
            }

            /**
		     * When references are changed, either null or another one, we expect dirty checking to be triggered (both sides
		     * have to be updated)
		     * When the internal object is changed, we apply the {Add|Update}Work on containedIns
		    */
            /*
		    if (searchForContainers)
			    processContainedIn(entity, queue, rootPropertiesMetadata, searchFactory);
		    */
        }

        /*
		private void processContainedIn(Object instance, List<LuceneWork> queue, PropertiesMetadata metadata, SearchFactory searchFactory)
		{
			not supported
		}
	    */

        private void ProcessContainedInValue(object value, List<LuceneWork> queue, System.Type valueClass,
                                             DocumentBuilder builder, SearchFactory searchFactory)
        {
            object id = GetMemberValue(value, builder.idGetter);
            builder.AddToWorkQueue(value, id, WorkType.Update, queue, searchFactory);
        }

        public Document GetDocument(object instance, object id)
        {
            Document doc = new Document();
            System.Type instanceClass = instance.GetType();
            if (rootPropertiesMetadata.boost != null)
            {
                doc.SetBoost(rootPropertiesMetadata.boost.Value);
            }
            // TODO: Check if that should be an else?
            {
                Field classField =
                    new Field(CLASS_FIELDNAME, instanceClass.AssemblyQualifiedName, Field.Store.YES,
                              Field.Index.UN_TOKENIZED);
                doc.Add(classField);
                idBridge.Set(idKeywordName, id, doc, Field.Store.YES, Field.Index.UN_TOKENIZED, idBoost);
            }
            BuildDocumentFields(instance, doc, rootPropertiesMetadata);
            return doc;
        }

        private static void BuildDocumentFields(Object instance, Document doc, PropertiesMetadata propertiesMetadata)
        {
            if (instance == null) return;

            for (int i = 0; i < propertiesMetadata.fieldNames.Count; i++)
            {
                MemberInfo member = propertiesMetadata.fieldGetters[i];
                Object value = GetMemberValue(instance, member);
                propertiesMetadata.fieldBridges[i].Set(
                    propertiesMetadata.fieldNames[i], value, doc, propertiesMetadata.fieldStore[i],
                    propertiesMetadata.fieldIndex[i], GetBoost(member)
                    );
            }

            for (int i = 0; i < propertiesMetadata.embeddedGetters.Count; i++)
            {
                MemberInfo member = propertiesMetadata.embeddedGetters[i];
                Object value = GetMemberValue(instance, member);
                //if ( ! Hibernate.isInitialized( value ) ) continue; //this sounds like a bad idea 
                //TODO handle boost at embedded level: already stored in propertiesMedatada.boost
                BuildDocumentFields(value, doc, propertiesMetadata.embeddedPropertiesMetadata[i]);
            }
        }

        public Term GetTerm(object id)
        {
            return new Term(idKeywordName, idBridge.ObjectToString(id));
        }

        public IDirectoryProvider DirectoryProvider
        {
            get { return directoryProvider; }
        }

        public Analyzer Analyzer
        {
            get { return analyzer; }
        }


        public ITwoWayFieldBridge IdBridge
        {
            get { return idBridge; }
        }

        public String getIdKeywordName()
        {
            return idKeywordName;
        }

        public static System.Type GetDocumentClass(Document document)
        {
            String className = document.Get(CLASS_FIELDNAME);
            try
            {
                return ReflectHelper.ClassForName(className);
            }
            catch (Exception e)
            {
                throw new SearchException("Unable to load indexed class: " + className, e);
            }
        }

        public static object GetDocumentId(SearchFactory searchFactory, Document document)
        {
            System.Type clazz = GetDocumentClass(document);
            DocumentBuilder builder = searchFactory.DocumentBuilders[clazz];
            if (builder == null) throw new SearchException("No Lucene configuration set up for: " + clazz.Name);
            return builder.IdBridge.Get(builder.getIdKeywordName(), document);
        }

        public void PostInitialize(ISet<System.Type> indexedClasses)
        {
            //this method does not requires synchronization
            System.Type plainClass = beanClass;
            ISet<System.Type> tempMappedSubclasses = new HashedSet<System.Type>();
            //together with the caller this creates a o(2), but I think it's still faster than create the up hierarchy for each class
            foreach (System.Type currentClass in indexedClasses)
            {
                if (plainClass.IsAssignableFrom(currentClass))
                    tempMappedSubclasses.Add(currentClass);
            }
            mappedSubclasses = tempMappedSubclasses;
        }

        public ISet<System.Type> MappedSubclasses
        {
            get { return mappedSubclasses; }
        }


        private class PropertiesMetadata
        {
            public float? boost = null;
            public readonly List<String> fieldNames = new List<String>();
            public readonly List<MemberInfo> fieldGetters = new List<MemberInfo>();
            public readonly List<IFieldBridge> fieldBridges = new List<IFieldBridge>();
            public readonly List<Field.Store> fieldStore = new List<Field.Store>();
            public readonly List<Field.Index> fieldIndex = new List<Field.Index>();
            public readonly List<MemberInfo> embeddedGetters = new List<MemberInfo>();
            public readonly List<PropertiesMetadata> embeddedPropertiesMetadata = new List<PropertiesMetadata>();
            public readonly List<MemberInfo> containedInGetters = new List<MemberInfo>();
        }
    }
}