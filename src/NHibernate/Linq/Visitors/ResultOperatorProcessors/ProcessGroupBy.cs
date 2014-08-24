using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;
using System.Linq;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessGroupBy : IResultOperatorProcessor<GroupResultOperator>
    {
        public void Process(GroupResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
			IEnumerable<Expression> groupByKeys;
			if (resultOperator.KeySelector is NewExpression)
				groupByKeys = (resultOperator.KeySelector as NewExpression).Arguments;
			else
				groupByKeys = new[] {resultOperator.KeySelector};

			IEnumerable<HqlExpression> hqlGroupByKeys = groupByKeys.Select(k => HqlGeneratorExpressionTreeVisitor.Visit(k, queryModelVisitor.VisitorParameters).AsExpression());

        	tree.AddGroupByClause(tree.TreeBuilder.GroupBy(hqlGroupByKeys.ToArray()));
        }
    }
}