using System.Collections;
using System.Linq;
using NHibernate.Hql.Ast;
using NHibernate.Hql.Ast.ANTLR;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessContains : IResultOperatorProcessor<ContainsResultOperator>
	{
		public void Process(ContainsResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var itemExpression =
				HqlGeneratorExpressionVisitor.Visit(resultOperator.Item, queryModelVisitor.VisitorParameters)
					.AsExpression();

			var from = GetFromRangeClause(tree.Root);
			var source = from.Children.First();

			if (source is HqlParameter)
			{
				// This is an "in" style statement
				if (IsEmptyList((HqlParameter)source, queryModelVisitor.VisitorParameters))
				{
					// if the list is empty the expression will always be false, so generate "1 = 0"
					tree.SetRoot(tree.TreeBuilder.Equality(tree.TreeBuilder.Constant(1), tree.TreeBuilder.Constant(0)));
				}
				else
				{
					tree.SetRoot(tree.TreeBuilder.In(itemExpression, source));
				}
			}
			else
			{
				// This is an "exists" style statement
				if (itemExpression is HqlParameter)
				{
					tree.AddWhereClause(tree.TreeBuilder.Equality(
						GetSelectExpression(tree.Root),
						itemExpression));
					ProcessAny.Process(tree);
				}
				else
				{
					tree.SetRoot(tree.TreeBuilder.In(itemExpression, tree.Root));
				}
			}
		}

		private static HqlRange GetFromRangeClause(HqlTreeNode node)
		{
			return node.NodesPreOrder.OfType<HqlRange>().First();
		}

		private static HqlExpression GetSelectExpression(HqlTreeNode node)
		{
			return node.NodesPreOrder.First(x => x.AstNode.Type == HqlSqlWalker.SELECT).Children.Single() as HqlExpression;
		}

		private static bool IsEmptyList(HqlParameter source, VisitorParameters parameters)
		{
			var parameterName = source.NodesPreOrder.Single(n => n is HqlIdent).AstNode.Text;
			// Multiple constants may be linked to the same parameter, take the first matching parameter
			var parameterValue = parameters.ConstantToParameterMap.First(p => p.Value.Name == parameterName).Key.Value;
			return !((IEnumerable)parameterValue).Cast<object>().Any();
		}
	}
}
