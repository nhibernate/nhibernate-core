using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;
using System.Linq;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAll : IResultOperatorProcessor<AllResultOperator>
    {
        public void Process(AllResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            tree.AddWhereClause(tree.TreeBuilder.BooleanNot(
                               HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Predicate, queryModelVisitor.VisitorParameters).
                                   AsBooleanExpression()));

			ReplaceSelect(tree, tree.TreeBuilder.Constant(1));

            tree.SetRoot(tree.TreeBuilder.BooleanNot(tree.TreeBuilder.Exists((HqlQuery) tree.Root)));
        }

		private static void ReplaceSelect(IntermediateHqlTree tree,HqlTreeNode replacement)
		{
			var selectFrom = tree.Root.Children.FirstOrDefault().As<HqlSelectFrom>();
			if (selectFrom != null)
			{
				var select = selectFrom.Children.OfType<HqlSelect>().SingleOrDefault();
				if (select != null && select.Children.Count() == 1)
				{
					select.ClearChildren();
					select.AddChild(replacement);
				}
			}
		}
    }
}