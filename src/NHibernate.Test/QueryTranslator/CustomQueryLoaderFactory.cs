using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Loader.Hql;

namespace NHibernate.Test.QueryTranslator
{
	/// <summary>
	/// Custom query loader factory to test the functionality of custom query translator factory.
	/// </summary>
	internal sealed class CustomQueryLoaderFactory: IQueryLoaderFactory
	{
		public IQueryLoader Create(
			QueryTranslatorImpl queryTranslator, 
			ISessionFactoryImplementor factory, 
			SelectClause selectClause)
		{
			return new CustomQueryLoader(new QueryLoader(queryTranslator,
			                                             factory,
			                                             selectClause));
		}
	}
}
