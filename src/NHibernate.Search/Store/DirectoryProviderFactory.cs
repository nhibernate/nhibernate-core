using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Search.Attributes;
using NHibernate.Search.Engine;
using NHibernate.Search.Storage;
using NHibernate.Util;

namespace NHibernate.Search.Storage
{
    public class DirectoryProviderFactory
    {
        private const String LUCENE_PREFIX = "hibernate.search.";
        private const String LUCENE_DEFAULT = LUCENE_PREFIX + "default.";

        private const string DEFAULT_DIRECTORY_PROVIDER =
            "NHibernate.Search.Storage.FSDirectoryProvider, NHibernate.Search";

        public List<IDirectoryProvider> providers = new List<IDirectoryProvider>();

        public IDirectoryProvider CreateDirectoryProvider(System.Type entity, Configuration cfg,
                                                          SearchFactory searchFactory)
        {
            //get properties
            String directoryProviderName = GetDirectoryProviderName(entity, cfg);
            IDictionary indexProps = GetDirectoryProperties(cfg, directoryProviderName);

            //set up the directory
            String className = (string) indexProps["directory_provider"];
            if (StringHelper.IsEmpty(className))
            {
                className = DEFAULT_DIRECTORY_PROVIDER;
            }
            IDirectoryProvider provider = null;
            try
            {
                System.Type directoryClass = ReflectHelper.ClassForName(className);
                provider = (IDirectoryProvider) Activator.CreateInstance(directoryClass);
            }
            catch (Exception e)
            {
                throw new HibernateException("Unable to instanciate directory provider: " + className, e);
            }
            try
            {
                provider.Initialize(directoryProviderName, indexProps, searchFactory);
            }
            catch (Exception e)
            {
                throw new HibernateException("Unable to initialize: " + directoryProviderName, e);
            }
            int index = providers.IndexOf(provider);
            if (index != -1)
            {
                //share the same Directory provider for the same underlying store
                return (IDirectoryProvider) providers[index];
            }
            else
            {
                providers.Add(provider);
                return provider;
            }
        }

        private static IDictionary GetDirectoryProperties(Configuration cfg, String directoryProviderName)
        {
            IDictionary props = cfg.Properties;
            String indexName = LUCENE_PREFIX + directoryProviderName;
            IDictionary indexProps = new Hashtable();
            IDictionary indexSpecificProps = new Hashtable();
            foreach (DictionaryEntry entry in props)
            {
                String key = (String) entry.Key;
                if (key.StartsWith(LUCENE_DEFAULT))
                {
                    indexProps[key.Substring(LUCENE_DEFAULT.Length)] = entry.Value;
                }
                else if (key.StartsWith(indexName))
                {
                    indexSpecificProps[key.Substring(indexName.Length)] = entry.Value;
                }
            }
            foreach (DictionaryEntry indexSpecificProp in indexSpecificProps)
            {
                indexProps[indexSpecificProp.Key] = indexSpecificProp.Value;
            }
            return indexProps;
        }

        private static String GetDirectoryProviderName(System.Type clazz, Configuration cfg)
        {
            //get the most specialized (ie subclass > superclass) non default index name
            //if none extract the name from the most generic (superclass > subclass) [Indexed] class in the hierarchy
            PersistentClass pc = cfg.GetClassMapping(clazz);
            System.Type rootIndex = null;
            do
            {
                IndexedAttribute indexAnn = AttributeUtil.GetIndexed(pc.MappedClass);
                if (indexAnn != null)
                {
                    if (string.IsNullOrEmpty(indexAnn.Index) == false)
                    {
                        return indexAnn.Index;
                    }
                    else
                    {
                        rootIndex = pc.MappedClass;
                    }
                }
                pc = pc.Superclass;
            } while (pc != null);
            //there is nobody out there with a non default [Indexed(Index = "fo")]
            if (rootIndex != null)
            {
                return rootIndex.Name;
            }
            else
            {
                throw new HibernateException(
                    "Trying to extract the index name from a non @Indexed class: " + clazz);
            }
        }
    }
}