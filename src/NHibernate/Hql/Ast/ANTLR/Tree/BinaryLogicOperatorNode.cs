using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Parameters;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Contract for nodes representing binary operators.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public class BinaryLogicOperatorNode : HqlSqlWalkerNode, IBinaryOperatorNode
	{
		public BinaryLogicOperatorNode(IToken token) : base(token)
		{
		}

		public IASTNode LeftHandOperand
		{
			get { return GetChild(0);}
		}

		public IASTNode RightHandOperand
		{
			get { return GetChild(1);}
		}

		/// <summary>
		/// Performs the operator node initialization by seeking out any parameter
		/// nodes and setting their expected type, if possible.
		/// </summary>
		public virtual void Initialize()
		{
			IASTNode lhs = LeftHandOperand;
			if ( lhs == null ) 
			{
				throw new SemanticException( "left-hand operand of a binary operator was null" );
			}
			IASTNode rhs = RightHandOperand;
			if ( rhs == null ) 
			{
				throw new SemanticException( "right-hand operand of a binary operator was null" );
			}

			IType lhsType = ExtractDataType( lhs );
			IType rhsType = ExtractDataType( rhs );

			if ( lhsType == null ) 
			{
				lhsType = rhsType;
			}
			if ( rhsType == null ) 
			{
				rhsType = lhsType;
			}

			if ( typeof(IExpectedTypeAwareNode).IsAssignableFrom( lhs.GetType() ) ) 
			{
				( ( IExpectedTypeAwareNode ) lhs ).ExpectedType = rhsType;
			}
			if ( typeof(IExpectedTypeAwareNode).IsAssignableFrom( rhs.GetType() ) ) 
			{
				( ( IExpectedTypeAwareNode ) rhs ).ExpectedType = lhsType;
			}

			MutateRowValueConstructorSyntaxesIfNecessary( lhsType, rhsType );
		}

		protected void MutateRowValueConstructorSyntaxesIfNecessary(IType lhsType, IType rhsType) 
		{
			// TODO : this really needs to be delayed unitl after we definitively know all node types
			// where this is currently a problem is parameters for which where we cannot unequivocally
			// resolve an expected type
			ISessionFactoryImplementor sessionFactory = SessionFactoryHelper.Factory;

			if ( lhsType != null && rhsType != null ) 
			{
				int lhsColumnSpan = lhsType.GetColumnSpan( sessionFactory );
				if ( lhsColumnSpan != rhsType.GetColumnSpan( sessionFactory ) ) 
				{
					throw new TypeMismatchException(
							"left and right hand sides of a binary logic operator were incompatibile [" +
							lhsType.Name + " : "+ rhsType.Name + "]"
					);
				}
				if ( lhsColumnSpan > 1 ) 
				{
					// for dialects which are known to not support ANSI-SQL row-value-constructor syntax,
					// we should mutate the tree.
					if ( !sessionFactory.Dialect.SupportsRowValueConstructorSyntax) 
					{
						MutateRowValueConstructorSyntax( lhsColumnSpan );
					}
				}
			}
		}

		/**
		 * Mutate the subtree relating to a row-value-constructor to instead use
		 * a series of ANDed predicates.  This allows multi-column type comparisons
		 * and explicit row-value-constructor syntax even on databases which do
		 * not support row-value-constructor.
		 * <p/>
		 * For example, here we'd mutate "... where (col1, col2) = ('val1', 'val2) ..." to
		 * "... where col1 = 'val1' and col2 = 'val2' ..."
		 *
		 * @param valueElements The number of elements in the row value constructor list.
		 */
		private void MutateRowValueConstructorSyntax(int valueElements) 
		{
			// mutation depends on the types of nodes invloved...
			int comparisonType = Type;
			string comparisonText = Text;
			Type = HqlSqlWalker.AND;
			Text = "AND";

			String[] lhsElementTexts = ExtractMutationTexts( LeftHandOperand, valueElements );
			String[] rhsElementTexts = ExtractMutationTexts( RightHandOperand, valueElements );

			IParameterSpecification lhsEmbeddedCompositeParameterSpecification =
					LeftHandOperand == null || ( !(LeftHandOperand is ParameterNode))
							? null
							: ( ( ParameterNode ) LeftHandOperand ).HqlParameterSpecification;

			IParameterSpecification rhsEmbeddedCompositeParameterSpecification =
					RightHandOperand == null || ( !(RightHandOperand is ParameterNode))
							? null
							: ( ( ParameterNode ) RightHandOperand ).HqlParameterSpecification;

			IASTNode container = this;

			for ( int i = valueElements - 1; i > 0; i-- ) 
			{
				if ( i == 1 ) 
				{
					container.ClearChildren();

					container.AddChildren(
						ASTFactory.CreateNode(
							comparisonType, comparisonText,
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, lhsElementTexts[0]),
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, rhsElementTexts[0])
							),
						ASTFactory.CreateNode(
							comparisonType, comparisonText,
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, lhsElementTexts[1]),
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, rhsElementTexts[1])
							));

					// "pass along" our initial embedded parameter node(s) to the first generated
					// sql fragment so that it can be handled later for parameter binding...
					SqlFragment fragment = ( SqlFragment ) container.GetChild(0).GetChild(0);
					if ( lhsEmbeddedCompositeParameterSpecification != null ) {
						fragment.AddEmbeddedParameter( lhsEmbeddedCompositeParameterSpecification );
					}
					if ( rhsEmbeddedCompositeParameterSpecification != null ) {
						fragment.AddEmbeddedParameter( rhsEmbeddedCompositeParameterSpecification );
					}
				}
				else
				{
					container.ClearChildren();
					container.AddChildren(
						ASTFactory.CreateNode(HqlSqlWalker.AND, "AND"),
						ASTFactory.CreateNode(
							comparisonType, comparisonText,
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, lhsElementTexts[i]),
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, rhsElementTexts[i])
							));

					container = container.GetChild(0);
				}
			}
		}

		private static string[] ExtractMutationTexts(IASTNode operand, int count) 
		{
			if ( operand is ParameterNode ) 
			{
				string[] rtn = new string[count];
				for ( int i = 0; i < count; i++ ) 
				{
					rtn[i] = "?";
				}
				return rtn;
			}
			else if ( operand.Type == HqlSqlWalker.VECTOR_EXPR ) 
			{
				string[] rtn = new string[operand.ChildCount];

				for (int x = 0; x < operand.ChildCount; x++)
				{
					rtn[ x++ ] = operand.GetChild(x).Text;
					
				}
				return rtn;
			}
			else if ( operand is SqlNode ) 
			{
				string nodeText = operand.Text;

				if ( nodeText.StartsWith( "(" ) ) 
				{
					nodeText = nodeText.Substring( 1 );
				}
				if ( nodeText.EndsWith( ")" ) ) 
				{
					nodeText = nodeText.Substring( 0, nodeText.Length - 1 );
				}
				String[] splits = StringHelper.Split( ", ", nodeText );

				if ( count != splits.Length ) 
				{
					throw new HibernateException( "SqlNode's text did not reference expected number of columns" );
				}
				return splits;
			}
			else
			{
				throw new HibernateException( "dont know how to extract row value elements from node : " + operand );
			}
		}

		protected static IType ExtractDataType(IASTNode operand) 
		{
			IType type = null;

			if ( operand is SqlNode ) 
			{
				type = ( ( SqlNode ) operand ).DataType;
			}

			if ( type == null && operand is IExpectedTypeAwareNode ) 
			{
				type = ( ( IExpectedTypeAwareNode ) operand ).ExpectedType;
			}

			return type;
		}
	}
}
