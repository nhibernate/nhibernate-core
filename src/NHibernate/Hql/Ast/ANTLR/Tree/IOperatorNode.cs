using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Contract for nodes representing operators (logic or arithmetic).
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public interface IOperatorNode 
	{
		/// <summary>
		/// Called by the tree walker during hql-sql semantic analysis
		/// after the operator sub-tree is completely built.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Retrieves the data type for the overall operator expression.
		/// </summary>
		/// <returns>The expression's data type.</returns>
		IType DataType
		{ 
			get;
		}
	}
}
