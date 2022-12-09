using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq;
using NHibernate.Loader.Hql;
using NHibernate.Util;

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
		public IQueryTranslator[] CreateQueryTranslators(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
			return CreateQueryTranslators(queryExpression, queryExpression.Translate(factory, collectionRole != null), queryExpression.Key, collectionRole, shallow, filters, factory);
		}

		static IQueryTranslator[] CreateQueryTranslators(
			IQueryExpression queryExpression,
			IASTNode ast,
			string queryIdentifier,
			string collectionRole,
			bool shallow,
			IDictionary<string, IFilter> filters,
			ISessionFactoryImplementor factory)
		{
			var polymorphicParsers = AstPolymorphicProcessor.Process(ast, factory);

			var translators = polymorphicParsers
				.ToArray(hql => queryExpression is NhLinqExpression linqExpression
							? new QueryTranslatorImpl(queryIdentifier, 
							                          hql, 
							                          filters,
							                          factory, 
							                          CreateQueryLoader,
							                          linqExpression.GetNamedParameterTypes())
							: new QueryTranslatorImpl(queryIdentifier, 
							                          hql, 
							                          filters, 
							                          factory, 
							                          CreateQueryLoader));

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

		/// <summary>
		/// Creates a query loader.
		/// </summary>
		/// <param name="queryTranslatorImpl"></param>
		/// <param name="sessionFactoryImplementor"></param>
		/// <param name="selectClause"></param>
		/// <returns></returns>
		private static IQueryLoader CreateQueryLoader(QueryTranslatorImpl queryTranslatorImpl, 
		                                              ISessionFactoryImplementor sessionFactoryImplementor,
		                                              SelectClause selectClause)
		{
			return new QueryLoader(queryTranslatorImpl,
			                       sessionFactoryImplementor,
			                       selectClause);
		}
	}
}
