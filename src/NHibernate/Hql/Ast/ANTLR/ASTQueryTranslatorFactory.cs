using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
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
			return CreateQueryTranslators(queryExpression.Translate(factory, collectionRole != null), queryExpression.Key, collectionRole, shallow, filters, factory);
		}

		static IQueryTranslator[] CreateQueryTranslators(IASTNode ast, string queryIdentifier, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
			var polymorphicParsers = AstPolymorphicProcessor.Process(ast, factory);

			var translators = polymorphicParsers
				.ToArray(hql => new QueryTranslatorImpl(queryIdentifier, hql, filters, factory));

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
