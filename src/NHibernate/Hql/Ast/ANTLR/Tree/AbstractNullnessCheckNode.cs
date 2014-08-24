using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Base class for nodes dealing 'is null' and 'is not null' operators.
	/// todo : a good deal of this is copied from BinaryLogicOperatorNode; look at consolidating these code fragments
	/// 
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public abstract class AbstractNullnessCheckNode : UnaryLogicOperatorNode 
	{
		protected AbstractNullnessCheckNode(IToken token) : base(token)
		{
		}

		public override void Initialize() 
		{
			// TODO : this really needs to be delayed unitl after we definitively know the operand node type;
			// where this is currently a problem is parameters for which where we cannot unequivocally
			// resolve an expected type
			IType operandType = ExtractDataType( Operand );

			if ( operandType == null ) 
			{
				return;
			}

			ISessionFactoryImplementor sessionFactory = SessionFactoryHelper.Factory;
			int operandColumnSpan = operandType.GetColumnSpan( sessionFactory );

			if ( operandColumnSpan > 1 ) 
			{
				MutateRowValueConstructorSyntax( operandColumnSpan );
			}
		}

		/// <summary>
		/// When (if) we need to expand a row value constructor, what is the type of connector to use between the
		/// expansion fragments.
		/// </summary>
		/// <returns>The expansion connector type.</returns>
		protected abstract int ExpansionConnectorType { get; }

		/// <summary>
		/// When (if) we need to expand a row value constructor, what is the text of connector to use between the
		/// expansion fragments.
		/// </summary>
		/// <returns>The expansion connector text.</returns>
		protected abstract string ExpansionConnectorText { get; }

		private void MutateRowValueConstructorSyntax(int operandColumnSpan)
		{
			int comparisonType = Type;
			string comparisonText = Text;

			int expansionConnectorType = ExpansionConnectorType;
			string expansionConnectorText = ExpansionConnectorText;

			Type = expansionConnectorType;
			Text = expansionConnectorText;

			String[] mutationTexts = ExtractMutationTexts( Operand, operandColumnSpan );

			IASTNode container = this;
			container.ClearChildren();

			for (int i = operandColumnSpan - 1; i > 0; i--) 
			{
				if ( i == 1 )
				{
					container.AddChildren(
						ASTFactory.CreateNode(comparisonType, comparisonText,
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, mutationTexts[0])),
						ASTFactory.CreateNode(comparisonType, comparisonText,
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, mutationTexts[1])));
				}
				else 
				{
					container.AddChildren(
						ASTFactory.CreateNode(expansionConnectorType, expansionConnectorText),
						ASTFactory.CreateNode(comparisonType, comparisonText,
							ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, mutationTexts[i])));

					container = GetChild(0);
				}
			}
		}

		private static IType ExtractDataType(IASTNode operand) 
		{
			IType type = null;
			var sqlNode = operand as SqlNode;
			if ( sqlNode != null ) 
			{
				type = sqlNode.DataType;
			}
			if (type == null)
			{
				var expectedTypeAwareNode = operand as IExpectedTypeAwareNode;
				if (expectedTypeAwareNode != null)
				{
					type = expectedTypeAwareNode.ExpectedType;
				}
			}
			return type;
		}

		private static string[] ExtractMutationTexts(IASTNode operand, int count) 
		{
			if ( operand is ParameterNode ) 
			{
				String[] rtn = new String[count];
				for ( int i = 0; i < count; i++ ) 
				{
					rtn[i] = "?";
				}
				return rtn;
			}
			else if ( operand.Type == HqlSqlWalker.VECTOR_EXPR ) 
			{
				string[] rtn = new string[ operand.ChildCount];
				int x = 0;

				foreach (IASTNode node in operand)
				{
					rtn[ x++ ] = node.Text;
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

				string[] splits = nodeText.Split(new[] { ", " }, StringSplitOptions.None);

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
	}
}
