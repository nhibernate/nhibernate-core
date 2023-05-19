using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Loader.Hql
{
	/// <summary>
	/// Creates query loaders.
	/// </summary>
	public interface IQueryLoaderFactory
	{
		/// <summary>
		/// Creates a query loader.
		/// </summary>
		/// <param name="queryTranslator"></param>
		/// <param name="factory"></param>
		/// <param name="selectClause"></param>
		/// <returns></returns>
		IQueryLoader Create(QueryTranslatorImpl queryTranslator, ISessionFactoryImplementor factory, SelectClause selectClause); 
	}
}
