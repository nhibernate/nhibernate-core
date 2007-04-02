using System;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Search.Impl;

namespace NHibernate.Search
{
	public static class Search
	{
		public static IFullTextSession CreateFullTextSession(ISession session)
		{
			ISessionImplementor sessionImplementor = (ISessionImplementor)session;
			if((sessionImplementor.Interceptor is SearchInterceptor)==false)
			{
				throw new HibernateException(@"
The session interceptor was not a SearchInterceptor.
In order to use Full Text Query, you must open the session with a SearchInterceptor. Like this:
sessionFactory.OpenSession(new SearchInterceptor());
");
			}
			return new FullTextSessionImpl(session);
		}

		public static ICriterion Query(Query luecneQuery)
		{
			return new LuceneQueryExpression(luecneQuery);
		}

		public static ICriterion Query(string luceneQuery)
		{
			QueryParser parser = new QueryParser("",new StandardAnalyzer());
			Query query = parser.Parse(luceneQuery);
			return Query(query);
		}
	}
}