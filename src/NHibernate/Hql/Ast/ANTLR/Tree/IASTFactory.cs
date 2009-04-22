namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	public interface IASTFactory
	{
		IASTNode CreateNode(int type, string text, params IASTNode[] children);
	}
}
