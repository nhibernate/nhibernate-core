using System;
using System.Collections;
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
		/// <param name="queryIdentifier">
		/// The query-identifier (used in <see cref="NHibernate.Stat.QueryStatistics"/> collection). 
		/// This is typically the same as the queryString parameter except for the case of
		/// split polymorphic queries which result in multiple physical sql queries.
		/// </param>
		/// <param name="queryString">The query string to be translated</param>
		/// <param name="filters">Currently enabled filters</param>
		/// <param name="factory">The session factory</param>
		/// <returns>An appropriate translator.</returns>
		IQueryTranslator CreateQueryTranslator(string queryIdentifier, string queryString, IDictionary filters, ISessionFactoryImplementor factory);

		/// <summary>
		/// Construct a <see cref="NHibernate.Hql.IFilterTranslator"/> instance capable of 
		/// translating an HQL filter string.
		/// </summary>
		/// <param name="queryIdentifier">
		/// The query-identifier (used in <see cref="NHibernate.Stat.QueryStatistics"/> collection). 
		/// This is typically the same as the queryString parameter except for the case of
		/// split polymorphic queries which result in multiple physical sql queries.
		/// </param>
		/// <param name="queryString">The query string to be translated</param>
		/// <param name="filters">Currently enabled filters</param>
		/// <param name="factory">The session factory</param>
		/// <returns>An appropriate translator.</returns>
		IFilterTranslator CreateFilterTranslator(string queryIdentifier, string queryString, IDictionary filters, ISessionFactoryImplementor factory);
	}
}