using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessAll : IResultOperatorProcessor<AllResultOperator>
	{
		public void Process(AllResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddWhereClause(tree.TreeBuilder.BooleanNot(
				HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Predicate, queryModelVisitor.VisitorParameters).
					ToBooleanExpression()));

			if (tree.IsRoot)
			{
				tree.AddTakeClause(tree.TreeBuilder.Constant(1));

				Expression<Func<IEnumerable<object>, bool>> x = l => !l.Any();
				tree.AddListTransformer(x);
			}
			else
			{
				tree.SetRoot(tree.TreeBuilder.BooleanNot(tree.TreeBuilder.Exists((HqlQuery)tree.Root)));
			}
		}
	}
}