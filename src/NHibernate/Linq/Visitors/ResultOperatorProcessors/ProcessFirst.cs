using System.Linq;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessFirst : ProcessFirstOrSingleBase, IResultOperatorProcessor<FirstResultOperator>
	{
		private static readonly MethodInfo FirstOrDefault =
			ReflectHelper.GetMethodDefinition(() => Queryable.FirstOrDefault<object>(null));
		private static readonly MethodInfo First =
			ReflectHelper.GetMethodDefinition(() => Queryable.First<object>(null));

		public void Process(FirstResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var firstMethod = resultOperator.ReturnDefaultWhenEmpty ? FirstOrDefault : First;

			AddClientSideEval(firstMethod, queryModelVisitor, tree);

			tree.AddTakeClause(tree.TreeBuilder.Constant(1));
		}
	}
}