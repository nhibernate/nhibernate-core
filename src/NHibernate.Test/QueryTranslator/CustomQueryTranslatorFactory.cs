using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Linq;
using NHibernate.Loader.Hql;
using NHibernate.Type;

namespace NHibernate.Test.QueryTranslator
{
	internal sealed class CustomQueryTranslatorFactory: IQueryTranslatorFactory
	{
		public IQueryTranslator[] CreateQueryTranslators(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> filters, ISessionFactoryImplementor factory)
		{
			return CreateQueryTranslators(queryExpression, 
			                              queryExpression.Translate(factory, collectionRole != null), 
			                              queryExpression.Key, 
			                              collectionRole,
			                              shallow, 
			                              filters, 
			                              factory);
		}

		private static IQueryTranslator[] CreateQueryTranslators(
			IQueryExpression queryExpression,
			IASTNode ast,
			string queryIdentifier,
			string collectionRole,
			bool shallow,
			IDictionary<string, IFilter> filters,
			ISessionFactoryImplementor factory)
		{
			var polymorphicParsers = AstPolymorphicProcessor.Process(ast, factory);

			IQueryTranslator[] translators = new IQueryTranslator[polymorphicParsers.Length];
			for(int i = 0; i < polymorphicParsers.Length; i++)
			{
				var parser = polymorphicParsers[i];

				IFilterTranslator translator = CreateTranslator(queryIdentifier,
				                                                filters,
				                                                factory,
				                                                parser,
				                                                queryExpression);
				if(collectionRole == null)
				{
					translator.Compile(factory.Settings.QuerySubstitutions, shallow);
				}
				else
				{
					translator.Compile(collectionRole, factory.Settings.QuerySubstitutions, shallow);
				}

				translators[i] = translator;
			}

			return translators;
		}
		
		private static QueryTranslatorImpl CreateTranslator(
			string queryIdentifier,
			IDictionary<string, IFilter> filters,
			ISessionFactoryImplementor factory,
			IASTNode parser,
			IQueryExpression queryExpression)
		{
			IDictionary<string, System.Tuple<IType, bool>> namedParameterTypes = new Dictionary<string, System.Tuple<IType, bool>>();

			if(queryExpression is ILinqQueryExpression linqQueryExpression)
			{
				namedParameterTypes = linqQueryExpression.GetNamedParameterTypes();
			}
			
			return new QueryTranslatorImpl(queryIdentifier,
			                               parser,
			                               filters,
			                               factory,
			                               new CustomQueryLoaderFactory(),
			                               namedParameterTypes);
		}
	}
}
