using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Hql.Classic
{
	/// <summary>
	/// Generates translators which uses the older hand-written parser to perform the translation.
	/// </summary>
	public class ClassicQueryTranslatorFactory: IQueryTranslatorFactory
	{
		#region IQueryTranslatorFactory Members

		public IQueryTranslator CreateQueryTranslator(string queryString, IDictionary filters, ISessionFactoryImplementor factory)
		{
			return new QueryTranslator(factory, queryString, filters);
		}

		public IFilterTranslator CreateFilterTranslator(string queryString, IDictionary filters, ISessionFactoryImplementor factory)
		{
			return new QueryTranslator(factory, queryString, filters);
		}

		#endregion
	}
}
