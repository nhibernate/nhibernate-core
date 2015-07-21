using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Interface for nodes which wish to be made aware of any determined "expected
	/// type" based on the context within they appear in the query.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public interface IExpectedTypeAwareNode
	{
		IType ExpectedType { get; set; }
	}
}
