using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;
using System.Linq;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessAny : IResultOperatorProcessor<AnyResultOperator>
    {
        public void Process(AnyResultOperator anyOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
			ReplaceSelect(tree, tree.TreeBuilder.Constant(1));
			tree.SetRoot(tree.TreeBuilder.Exists((HqlQuery) tree.Root));
        }

		private static void ReplaceSelect(IntermediateHqlTree tree, HqlTreeNode replacement)
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