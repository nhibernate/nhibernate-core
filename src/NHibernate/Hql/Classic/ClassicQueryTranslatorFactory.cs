using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Hql.Classic
{
	/// <summary>
	/// Generates translators which uses the older hand-written parser to perform the translation.
	/// </summary>
	public class ClassicQueryTranslatorFactory : IQueryTranslatorFactory
	{
		public IQueryTranslator[] CreateQueryTranslators(string queryString, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
		    var translators = QuerySplitter.ConcreteQueries(queryString, factory)
                                           .Select(hql => new QueryTranslator(queryString, hql, filters, factory))
	                                       .ToArray();

            foreach (var translator in translators)
            {
                if (collectionRole == null)
                {
                    translator.Compile(factory.Settings.QuerySubstitutions, shallow);
                }
                else
                {
                    translator.Compile(collectionRole, factory.Settings.QuerySubstitutions, shallow);
                }
            }

		    return translators;
		}
	}
}