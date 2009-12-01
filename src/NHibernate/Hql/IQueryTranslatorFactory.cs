using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
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
		/// capable of translating an HQL query string.
		/// </summary>
		/// <param name="queryString">The query string to be translated</param>
		/// <param name="filters">Currently enabled filters</param>
		/// <param name="factory">The session factory</param>
		/// <returns>An appropriate translator.</returns>
        IQueryTranslator[] CreateQueryTranslators(string queryString, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory);
	}

    /// <summary>
    /// Facade for generation of <see cref="NHibernate.Hql.IQueryTranslator"/> 
    /// and <see cref="NHibernate.Hql.IFilterTranslator"/> instances.
    /// </summary>
    public interface IQueryTranslatorFactory2 : IQueryTranslatorFactory
    {
        /// <summary>
        /// Construct a <see cref="NHibernate.Hql.IQueryTranslator"/> instance 
        /// capable of translating a Linq expression.
        /// </summary>
        /// <param name="queryIdentifier">
        /// The query-identifier (used in <see cref="NHibernate.Stat.QueryStatistics"/> collection). 
        /// This is typically the same as the queryString parameter except for the case of
        /// split polymorphic queries which result in multiple physical sql queries.
        /// </param>
        /// <param name="queryExpression">The query expression to be translated</param>
        /// <param name="filters">Currently enabled filters</param>
        /// <param name="factory">The session factory</param>
        /// <returns>An appropriate translator.</returns>
        IQueryTranslator[] CreateQueryTranslators(string queryIdentifier, IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory);
    }
}