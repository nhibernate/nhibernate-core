using System.IO;
#if NET_2_0
using System.Collections.Generic;
using Iesi.Collections.Generic;
#else
using System.Collections;
using Iesi.Collections;
#endif
using Lucene.Net.Index;
using Lucene.Net.Search;
using NHibernate.Search.Engine;
using Directory=Lucene.Net.Store.Directory;

namespace NHibernate.Search
{
	public class FullTextSearchHelper
	{
#if NET_2_0
		public static Query FilterQueryByClasses(ISet<System.Type> classesAndSubclasses, Query luceneQuery)
#else
		public static Query FilterQueryByClasses(ISet classesAndSubclasses, Query luceneQuery)
#endif
		{
			//A query filter is more practical than a manual class filtering post query (esp on scrollable resultsets)
			//it also probably minimise the memory footprint
			if (classesAndSubclasses == null)
			{
				return luceneQuery;
			}
			else
			{
				BooleanQuery classFilter = new BooleanQuery();
				//annihilate the scoring impact of DocumentBuilder.CLASS_FIELDNAME
				classFilter.SetBoost(0);
				foreach (System.Type clazz in classesAndSubclasses)
				{
					Term t = new Term(DocumentBuilder.CLASS_FIELDNAME, clazz.AssemblyQualifiedName);
					TermQuery termQuery = new TermQuery(t);
					classFilter.Add(termQuery, BooleanClause.Occur.SHOULD);
				}
				BooleanQuery filteredQuery = new BooleanQuery();
				filteredQuery.Add(luceneQuery, BooleanClause.Occur.MUST);
				filteredQuery.Add(classFilter, BooleanClause.Occur.MUST);
				return filteredQuery;
			}
		}

#if NET_2_0
		public static Searcher BuildSearcher(SearchFactory searchFactory, out ISet<System.Type> classesAndSubclasses, params System.Type[] classes)
		{
			Dictionary<System.Type, DocumentBuilder> builders = searchFactory.DocumentBuilders;
			ISet<Directory> directories = new HashedSet<Directory>();
#else
		public static Searcher BuildSearcher(SearchFactory searchFactory, out ISet classesAndSubclasses, params System.Type[] classes)
		{
			Hashtable builders = searchFactory.DocumentBuilders;
			ISet directories = new HashedSet();
#endif
			if (classes == null || classes.Length == 0)
			{
				//no class means all classes
				foreach (DocumentBuilder builder in builders.Values)
				{
					directories.Add(builder.DirectoryProvider.Directory);
				}
				classesAndSubclasses = null;
			}
			else
			{
#if NET_2_0
				ISet<System.Type> involvedClasses = new HashedSet<System.Type>();
#else
				ISet involvedClasses = new HashedSet();
#endif
				involvedClasses.AddAll(classes);
				foreach (System.Type clazz in classes)
				{
					DocumentBuilder builder;
#if NET_2_0
					builders.TryGetValue(clazz, out builder);
#else
					builder = (DocumentBuilder) (builders.ContainsKey(clazz.Name) ? builders[clazz.Name] : null);
#endif
					if (builder != null) involvedClasses.AddAll(builder.MappedSubclasses);
				}
				foreach (System.Type clazz in involvedClasses)
				{
					DocumentBuilder builder;
#if NET_2_0
					builders.TryGetValue(clazz, out builder);
#else
					builder = (DocumentBuilder) (builders.ContainsKey(clazz.Name) ? builders[clazz.Name] : null);
#endif
					//TODO should we rather choose a polymorphic path and allow non mapped entities
					if (builder == null) throw new HibernateException("Not a mapped entity: " + clazz);
					directories.Add(builder.DirectoryProvider.Directory);
				}
				classesAndSubclasses = involvedClasses;
			}

			return GetSearcher(directories);
		}

#if NET_2_0
		public static Searcher GetSearcher(ISet<Directory> directories)
#else
		public static Searcher GetSearcher(ISet directories)
#endif
		{
			if (directories.Count == 0)
				return null;
			//set up the searcher
			int dirNbr = directories.Count;
			IndexSearcher[] searchers = new IndexSearcher[dirNbr];
			try
			{
				int index = 0;
				foreach (Directory directory in directories)
				{
					if (dirNbr == 1)
						return new IndexSearcher(directory);
					searchers[index] = new IndexSearcher(directory);
					index += 1;
				}
				return new MultiSearcher(searchers);
			}
			catch (IOException e)
			{
				throw new HibernateException("Unable to read Lucene directory", e);
			}
		}
	}
}