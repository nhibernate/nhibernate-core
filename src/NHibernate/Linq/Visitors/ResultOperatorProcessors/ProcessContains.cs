using System.Collections;
using System.Linq;
using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessContains : IResultOperatorProcessor<ContainsResultOperator>
	{
		public void Process(ContainsResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var itemExpression =
				HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Item, queryModelVisitor.VisitorParameters)
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
						tree.TreeBuilder.Ident(GetFromAlias(tree.Root).AstNode.Text),
						itemExpression));
					tree.SetRoot(tree.TreeBuilder.Exists((HqlQuery)tree.Root));
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

		private static HqlAlias GetFromAlias(HqlTreeNode node)
		{
			return node.NodesPreOrder.Single(n => n is HqlRange).Children.Single(n => n is HqlAlias) as HqlAlias;
		}

		private static bool IsEmptyList(HqlParameter source, VisitorParameters parameters)
		{
			var parameterName = source.NodesPreOrder.Single(n => n is HqlIdent).AstNode.Text;
			var parameterValue = parameters.ConstantToParameterMap.Single(p => p.Value.Name == parameterName).Key.Value;
			return !((IEnumerable)parameterValue).Cast<object>().Any();
		}
	}
}