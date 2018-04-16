using System;
using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Hql
{
	/// <summary>
	/// Facade for generation of <see cref="NHibernate.Hql.IQueryTranslator"/> 
	/// and <see cref="NHibernate.Hql.IFilterTranslator"/> instances.
	/// </summary>
	public interface IQueryTranslatorFactory
	{
		/// <summary>
		/// Construct a <see cref="NHibernate.Hql.IQueryTranslator"/> instance 
		/// capable of translating a Linq expression.
		/// </summary>
		/// <param name="queryExpression">The query expression to be translated</param>
		/// <param name="collectionRole"></param>
		/// <param name="shallow"></param>
		/// <param name="filters">Currently enabled filters</param>
		/// <param name="factory">The session factory</param>
		/// <returns>An appropriate translator.</returns>
		IQueryTranslator[] CreateQueryTranslators(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory);
	}
}
