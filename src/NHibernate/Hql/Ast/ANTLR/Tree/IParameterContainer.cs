using NHibernate.Param;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Currently this is needed in order to deal with {@link FromElement FromElements} which
	/// contain "hidden" JDBC parameters from applying filters.
	/// Would love for this to go away, but that would require that Hibernate's
	/// internal {@link org.hibernate.engine.JoinSequence join handling} be able to either:<ul>
	/// <li>render the same AST structures</li>
	/// <li>render structures capable of being converted to these AST structures</li>
	/// </ul>
	/// In the interim, this allows us to at least treat these "hidden" parameters properly which is
	/// the most pressing need.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public interface IParameterContainer
	{
		/// <summary>
		/// Set the renderable text of this node.
		/// </summary>
		string Text { set; }

		/// <summary>
		/// Adds a parameter specification for a parameter encountered within this node.  We use the term 'embedded' here
		/// because of the fact that the parameter was simply encountered as part of the node's text; it does not exist
		/// as part of a subtree as it might in a true AST.
		/// </summary>
		/// <param name="specification">The generated specification.</param>
		void AddEmbeddedParameter(IParameterSpecification specification);

		/// <summary>
		/// Determine whether this node contans embedded parameters.  The implication is that
		/// {@link #getEmbeddedParameters()} is allowed to return null if this method returns false.
		/// </summary>
		bool HasEmbeddedParameters
		{ 
			get;
		}

		/// <summary>
		/// Retrieve all embedded parameter specifications.
		/// </summary>
		/// <returns>All embedded parameter specifications; may return null.</returns>
		IParameterSpecification[] GetEmbeddedParameters();
	}
}
