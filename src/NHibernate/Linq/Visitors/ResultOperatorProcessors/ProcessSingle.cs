using System.Linq;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessSingle : ProcessFirstOrSingleBase, IResultOperatorProcessor<SingleResultOperator>
	{
		private static readonly MethodInfo SingleOrDefault =
			ReflectionCache.QueryableMethods.SingleOrDefaultDefinition;
		private static readonly MethodInfo Single =
			ReflectionCache.QueryableMethods.SingleDefinition;

		public void Process(SingleResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var firstMethod = resultOperator.ReturnDefaultWhenEmpty ? SingleOrDefault : Single;

			AddClientSideEval(firstMethod, queryModelVisitor, tree);
		}
	}
}