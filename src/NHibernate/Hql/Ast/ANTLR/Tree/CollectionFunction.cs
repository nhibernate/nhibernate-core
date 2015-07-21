using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents 'elements()' or 'indices()'.
	/// Author: josh
	/// Ported by: Steve strong
	/// </summary>
	[CLSCompliant(false)]
	public class CollectionFunction : MethodNode, IDisplayableNode 
	{
		public CollectionFunction(IToken token) : base(token)
		{
		}

		public override void Resolve(bool inSelect) 
		{
			InitializeMethodNode(this, inSelect);

			if ( !IsCollectionPropertyMethod ) 
			{
				throw new SemanticException( Text + " is not a collection property name!" );
			}
			IASTNode expr = GetChild(0);

			if ( expr == null ) 
			{
				throw new SemanticException( Text + " requires a path!" );
			}

			ResolveCollectionProperty( expr );
		}

		protected override void PrepareSelectColumns(String[] selectColumns) 
		{
			// we need to strip off the embedded parens so that sql-gen does not double these up
			String subselect = selectColumns[0].Trim();
			if ( subselect.StartsWith( "(") && subselect.EndsWith( ")" ) ) 
			{
				subselect = subselect.Substring( 1, subselect.Length -2 );
			}
			selectColumns[0] = subselect;
		}
	}
}
