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

				Expression<Func<IEnumerable<object>, object[], bool>> x = (l, p) => l.Any();
				tree.AddListTransformer(x);

				// NH-3850: Queries with polymorphism yields many results which must be combined.
				Expression<Func<IEnumerable<bool>, object[], bool>> px = (l, p) => l.Any(r => r);
				tree.AddPostExecuteTransformer(px);
			}
			else
			{
				tree.SetRoot(tree.TreeBuilder.Exists((HqlQuery)tree.Root));
			}
		}
	}
}
