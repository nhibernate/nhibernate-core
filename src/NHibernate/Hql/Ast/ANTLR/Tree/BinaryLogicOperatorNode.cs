using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Contract for nodes representing binary operators.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class BinaryLogicOperatorNode : HqlSqlWalkerNode, IBinaryOperatorNode, IParameterContainer
	{
		private List<IParameterSpecification> embeddedParameters;

		public BinaryLogicOperatorNode(IToken token)
			: base(token)
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
			ProcessMetaTypeDiscriminatorIfNecessary(lhs, rhs);
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

			var lshExpectedTypeAwareNode = lhs as IExpectedTypeAwareNode;
			if (lshExpectedTypeAwareNode != null)
			{
				lshExpectedTypeAwareNode.ExpectedType = rhsType;
			}
			var rshExpectedTypeAwareNode = rhs as IExpectedTypeAwareNode;
			if (rshExpectedTypeAwareNode != null)
			{
				rshExpectedTypeAwareNode.ExpectedType = lhsType;
			}

			MutateRowValueConstructorSyntaxesIfNecessary( lhsType, rhsType );
		}

		protected void MutateRowValueConstructorSyntaxesIfNecessary(IType lhsType, IType rhsType)
		{
			// TODO : this really needs to be delayed unitl after we definitively know all node types
			// where this is currently a problem is parameters for which where we cannot unequivocally
			// resolve an expected type
			ISessionFactoryImplementor sessionFactory = SessionFactoryHelper.Factory;

			if (lhsType != null && rhsType != null)
			{
				int lhsColumnSpan = lhsType.GetColumnSpan(sessionFactory);
				var rhsColumnSpan = rhsType.GetColumnSpan(sessionFactory);
				// NH different behavior NH-1801
				if (lhsColumnSpan != rhsColumnSpan && !AreCompatibleEntityTypes(lhsType, rhsType))
				{
					throw new TypeMismatchException("left and right hand sides of a binary logic operator were incompatibile ["
					                                + lhsType.Name + " : " + rhsType.Name + "]");
				}
				if (lhsColumnSpan > 1)
				{
					// for dialects which are known to not support ANSI-SQL row-value-constructor syntax,
					// we should mutate the tree.
					if (!sessionFactory.Dialect.SupportsRowValueConstructorSyntax)
					{
						MutateRowValueConstructorSyntax(lhsColumnSpan);
					}
				}
			}
		}

		private static bool AreCompatibleEntityTypes(IType lhsType, IType rhsType)
		{
			if(lhsType.IsEntityType && rhsType.IsEntityType)
			{
				return lhsType.ReturnedClass.IsAssignableFrom(rhsType.ReturnedClass) ||
					rhsType.ReturnedClass.IsAssignableFrom(lhsType.ReturnedClass);
			}
			return false;
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
			// Reduce the new tree in just one SqlFragment, to manage parameters

			// mutation depends on the types of nodes invloved...
			string comparisonText = "==".Equals(Text) ? "=" : Text;
			Type = HqlSqlWalker.SQL_TOKEN;
			string[] lhsElementTexts = ExtractMutationTexts(LeftHandOperand, valueElements);
			string[] rhsElementTexts = ExtractMutationTexts(RightHandOperand, valueElements);

			var lho = LeftHandOperand as ParameterNode;
			IParameterSpecification lhsEmbeddedCompositeParameterSpecification = (lho == null) ? null : lho.HqlParameterSpecification;

			var rho = RightHandOperand as ParameterNode;
			IParameterSpecification rhsEmbeddedCompositeParameterSpecification = (rho == null) ? null : rho.HqlParameterSpecification;

			var multicolumnComparisonClause = Translate(valueElements, comparisonText, lhsElementTexts, rhsElementTexts);

			if (lhsEmbeddedCompositeParameterSpecification != null)
			{
				AddEmbeddedParameter(lhsEmbeddedCompositeParameterSpecification);
			}
			if (rhsEmbeddedCompositeParameterSpecification != null)
			{
				AddEmbeddedParameter(rhsEmbeddedCompositeParameterSpecification);
			}
			ClearChildren();
			Text = multicolumnComparisonClause;
		}

		public override SqlString RenderText(ISessionFactoryImplementor sessionFactory)
		{
			if(!HasEmbeddedParameters)
			{
				// this expression was not changed by MutateRowValueConstructorSyntax
				return base.RenderText(sessionFactory);
			}
			var result = SqlString.Parse(Text);
			// query-parameter = the parameter specified in the NHibernate query
			// sql-parameter = real parameter/s inside the final SQL
			// here is where we suppose the SqlString has all sql-parameters in sequence for a given query-parameter.
			// This happen when the query-parameter spans multiple columns (components,custom-types and so on).
			var parameters = result.GetParameters().ToArray();
			var sqlParameterPos = 0;
			var paramTrackers = embeddedParameters.SelectMany(specification => specification.GetIdsForBackTrack(sessionFactory));
			foreach (var paramTracker in paramTrackers)
			{
				parameters[sqlParameterPos++].BackTrack = paramTracker;
			}
			return result;
		}

		public void AddEmbeddedParameter(IParameterSpecification specification)
		{
			if (embeddedParameters == null)
			{
				embeddedParameters = new List<IParameterSpecification>();
			}
			embeddedParameters.Add(specification);
		}

		public bool HasEmbeddedParameters
		{
			get { return embeddedParameters != null && embeddedParameters.Count != 0; }
		}

		public IParameterSpecification[] GetEmbeddedParameters()
		{
			return embeddedParameters.ToArray();
		}

		private string Translate(int valueElements, string comparisonText, string[] lhsElementTexts, string[] rhsElementTexts)
		{
			var multicolumnComparisonClauses = new List<string>();
			for (int i = 0; i < valueElements; i++)
			{
				multicolumnComparisonClauses.Add(string.Format("{0} {1} {2}", lhsElementTexts[i], comparisonText, rhsElementTexts[i]));
			}
			return "(" + string.Join(" and ", multicolumnComparisonClauses.ToArray()) + ")";
		}

		private static string[] ExtractMutationTexts(IASTNode operand, int count) 
		{
			if ( operand is ParameterNode ) 
			{
				return Enumerable.Repeat("?", count).ToArray();
			}
			if (operand is SqlNode)
			{
				string nodeText = operand.Text;

				if (nodeText.StartsWith("("))
				{
					nodeText = nodeText.Substring(1);
				}
				if (nodeText.EndsWith(")"))
				{
					nodeText = nodeText.Substring(0, nodeText.Length - 1);
				}
				string[] splits = nodeText.Split(new[] { ", " }, StringSplitOptions.None);

				if (count != splits.Length)
				{
					throw new HibernateException("SqlNode's text did not reference expected number of columns");
				}
				return splits;
			}
			if (operand.Type == HqlSqlWalker.VECTOR_EXPR) 
			{
				var rtn = new string[operand.ChildCount];

				for (int x = 0; x < operand.ChildCount; x++)
				{
					rtn[ x ] = operand.GetChild(x).Text;
				}
				return rtn;
			}
			throw new HibernateException( "dont know how to extract row value elements from node : " + operand );
		}

		protected static IType ExtractDataType(IASTNode operand) 
		{
			IType type = null;

			var sqlNode = operand as SqlNode;
			if (sqlNode != null)
			{
				type = sqlNode.DataType;
			}

			var expectedTypeAwareNode = operand as IExpectedTypeAwareNode;
			if (type == null && expectedTypeAwareNode != null)
			{
				type = expectedTypeAwareNode.ExpectedType;
			}

			return type;
		}

		private void ProcessMetaTypeDiscriminatorIfNecessary(IASTNode lhs, IASTNode rhs)
		{
			// this method inserts the discriminator value for the rhs node so that .class queries on <any> mappings work with the class name
			var lhsNode = lhs as SqlNode;
			var rhsNode = rhs as SqlNode;
			if (lhsNode == null || rhsNode == null)
			{
				return;
			}

			var lhsNodeMetaType = lhsNode.DataType as MetaType;
			if (lhsNodeMetaType != null)
			{
				string className = SessionFactoryHelper.GetImportedClassName(rhsNode.OriginalText);

				object discriminatorValue = lhsNodeMetaType.GetMetaValue(TypeNameParser.Parse(className).Type);
				rhsNode.Text = discriminatorValue.ToString();
				return;
			}

			var rhsNodeMetaType = rhsNode.DataType as MetaType;
			if (rhsNodeMetaType != null)
			{
				string className = SessionFactoryHelper.GetImportedClassName(lhsNode.OriginalText);

				object discriminatorValue = rhsNodeMetaType.GetMetaValue(TypeNameParser.Parse(className).Type);
				lhsNode.Text = discriminatorValue.ToString();
				return;
			}
		}
	}
}
