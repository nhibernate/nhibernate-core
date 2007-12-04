using System;
#if NET_2_0
using System.Collections.Generic;
using Iesi.Collections.Generic;
#else
using System.Collections;
using Iesi.Collections;
#endif
using Lucene.Net.Search;
using NHibernate.Expression;
using NHibernate.Impl;
using NHibernate.Search.Engine;
using NHibernate.Search.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Search
{
	public class LuceneQueryExpression : InExpression
	{
		private readonly Query luceneQuery;

		public LuceneQueryExpression(Query luceneQuery)
			: base("id", new object[0])
		{
			this.luceneQuery = luceneQuery;
		}

		public override NHibernate.SqlCommand.SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, System.Collections.IDictionary enabledFilters)
		{
#if NET_2_0
			ISet<System.Type> types;
			List<object> ids = new List<object>();
#else
			ISet types;
			ArrayList ids = new ArrayList();
#endif

			System.Type type = GetCriteriaClass(criteria);
			SearchFactory searchFactory = SearchFactory.GetSearchFactory(GetSession(criteria));
			Searcher searcher = FullTextSearchHelper.BuildSearcher(searchFactory, out types, type);
			if (searcher == null)
				throw new SearchException("Could not find a searcher for class: " + type.FullName);
			Query query = FullTextSearchHelper.FilterQueryByClasses(types, luceneQuery);
			Hits hits = searcher.Search(query);

			for (int i = 0; i < hits.Length(); i++)
			{
				object id = DocumentBuilder.GetDocumentId(searchFactory,hits.Doc(i));
				ids.Add(id);
			}
			base.Values = ids.ToArray();
			return base.ToSqlString(criteria, criteriaQuery, enabledFilters);
		}

		private System.Type GetCriteriaClass(ICriteria criteria)
		{
			CriteriaImpl impl = criteria as CriteriaImpl;
			if (impl != null)
				return impl.CriteriaClass;
			return GetCriteriaClass(((CriteriaImpl.Subcriteria) criteria).Parent);
		}

		public ISession GetSession(ICriteria criteria)
		{
			CriteriaImpl impl = criteria as CriteriaImpl;
			if (impl != null)
				return impl.Session;
			return GetSession(((CriteriaImpl.Subcriteria) criteria).Parent);
		}
	}
}