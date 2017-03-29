using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessAny : IResultOperatorProcessor<AnyResultOperator>
	{
		public void Process(AnyResultOperator anyOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			if (tree.IsRoot)
			{
				tree.AddTakeClause(tree.TreeBuilder.Constant(1));

				Expression<Func<IEnumerable<object>, bool>> x = l => l.Any();
				tree.AddListTransformer(x);
			}
			else
			{
				tree.SetRoot(tree.TreeBuilder.Exists((HqlQuery)tree.Root));
			}
		}
	}
}