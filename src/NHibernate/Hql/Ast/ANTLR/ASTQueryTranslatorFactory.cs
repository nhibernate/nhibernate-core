using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Generates translators which uses the Antlr-based parser to perform
	/// the translation.
	/// 
	/// Author: Gavin King
	/// Ported by: Steve Strong
	/// </summary>
	public class ASTQueryTranslatorFactory : IQueryTranslatorFactory
	{
		public IQueryTranslator CreateQueryTranslator(string queryIdentifier, string queryString, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
			return new QueryTranslatorImpl(queryIdentifier, queryString, filters, factory);
		}

		public IFilterTranslator CreateFilterTranslator(string queryIdentifier, string queryString, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
			return new QueryTranslatorImpl(queryIdentifier, queryString, filters, factory);
		}
	}
}
