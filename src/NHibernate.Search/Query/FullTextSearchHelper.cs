using System.Collections.Generic;
using System.IO;
using Iesi.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search;
using NHibernate.Search.Impl;
using Directory=Lucene.Net.Store.Directory;

namespace NHibernate.Search
{
	public static class FullTextSearchHelper
	{
		public static Query FilterQueryByClasses(ISet<System.Type> classesAndSubclasses, Query luceneQuery)
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

		public static Searcher BuildSearcher(SearchFactory searchFactory, out ISet<System.Type> classesAndSubclasses, params System.Type[] classes)
		{
			Dictionary<System.Type, DocumentBuilder> builders = searchFactory.DocumentBuilders;
			ISet<Directory> directories = new HashedSet<Directory>();
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
				ISet<System.Type> involvedClasses = new HashedSet<System.Type>();
				involvedClasses.AddAll(classes);
				foreach (System.Type clazz in classes)
				{
					DocumentBuilder builder;
					builders.TryGetValue(clazz, out builder);
					if (builder != null) involvedClasses.AddAll(builder.MappedSubclasses);
				}
				foreach (System.Type clazz in involvedClasses)
				{
					DocumentBuilder builder;
					builders.TryGetValue(clazz, out builder);
					//TODO should we rather choose a polymorphic path and allow non mapped entities
					if (builder == null) throw new HibernateException("Not a mapped entity: " + clazz);
					directories.Add(builder.DirectoryProvider.Directory);
				}
				classesAndSubclasses = involvedClasses;
			}

			return GetSearcher(directories);
		}

		public static Searcher GetSearcher(ISet<Directory> directories)
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