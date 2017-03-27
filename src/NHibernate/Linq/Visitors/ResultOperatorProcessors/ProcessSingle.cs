using System.Linq;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessSingle : ProcessFirstOrSingleBase, IResultOperatorProcessor<SingleResultOperator>
	{
		private static readonly MethodInfo SingleOrDefault =
			ReflectHelper.GetMethodDefinition(() => Queryable.SingleOrDefault<object>(null));
		private static readonly MethodInfo Single =
			ReflectHelper.GetMethodDefinition(() => Queryable.Single<object>(null));

		public void Process(SingleResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var firstMethod = resultOperator.ReturnDefaultWhenEmpty ? SingleOrDefault : Single;

			AddClientSideEval(firstMethod, queryModelVisitor, tree);
		}
	}
}