using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
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

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
                                              IDictionary enabledFilters)
        {
            ISet<System.Type> types;
            List<object> ids = new List<object>();

            System.Type type = GetCriteriaClass(criteria);
            SearchFactory searchFactory = SearchFactory.GetSearchFactory(GetSession(criteria));
            Searcher searcher = FullTextSearchHelper.BuildSearcher(searchFactory, out types, type);
            if (searcher == null)
                throw new SearchException("Could not find a searcher for class: " + type.FullName);
            Query query = FullTextSearchHelper.FilterQueryByClasses(types, luceneQuery);
            Hits hits = searcher.Search(query);

            for (int i = 0; i < hits.Length(); i++)
            {
                object id = DocumentBuilder.GetDocumentId(searchFactory, hits.Doc(i));
                ids.Add(id);
            }
            base.Values = ids.ToArray();
            return base.ToSqlString(criteria, criteriaQuery, enabledFilters);
        }

        private static System.Type GetCriteriaClass(ICriteria criteria)
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