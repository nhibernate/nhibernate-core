using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using Lucene.Net.Search;
using NHibernate.Expression;
using NHibernate.Impl;
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
			System.Type type = GetCriteriaClass(criteria);
			SearchFactory searchFactory = SearchFactory.GetSearchFactory(GetSession(criteria));
			ISet<System.Type> types;
			Searcher searcher = FullTextSearchHelper.BuildSearcher(searchFactory, out types, type);
			if (searcher == null)
				throw new SearchException("Could not find a searcher for class: " + type.FullName);
			Query query = FullTextSearchHelper.FilterQueryByClasses(types, luceneQuery);
			Hits hits = searcher.Search(query);
			List<object> ids = new List<object>();
			for (int i = 0; i < hits.Length(); i++)
			{
				object id = DocumentBuilder.GetDocumentId(searchFactory,hits.Doc(i));
				ids.Add(id);
			}
			base._values = ids.ToArray();
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