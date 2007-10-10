using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Hql.Classic
{
	/// <summary>
	/// Generates translators which uses the older hand-written parser to perform the translation.
	/// </summary>
	public class ClassicQueryTranslatorFactory : IQueryTranslatorFactory
	{
		#region IQueryTranslatorFactory Members

		public IQueryTranslator CreateQueryTranslator(string queryIdentifier, string queryString, IDictionary<string, IFilter> filters,
		                                              ISessionFactoryImplementor factory)
		{
			return new QueryTranslator(queryIdentifier, queryString, filters, factory);
		}

		public IFilterTranslator CreateFilterTranslator(string queryIdentifier, string queryString, IDictionary<string, IFilter> filters,
		                                                ISessionFactoryImplementor factory)
		{
			return new QueryTranslator(queryIdentifier, queryString, filters, factory);
		}

		#endregion
	}
}