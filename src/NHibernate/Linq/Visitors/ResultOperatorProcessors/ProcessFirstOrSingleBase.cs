using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessFirstOrSingleBase
	{
		protected static void AddClientSideEval(MethodInfo target, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var type = queryModelVisitor.Model.SelectClause.Selector.Type;
			target = target.MakeGenericMethod(type);

			var parameter = Expression.Parameter(typeof(IQueryable<>).MakeGenericType(type), null);

			var lambda = Expression.Lambda(
				Expression.Call(
					target,
					parameter),
				parameter);

			tree.AddPostExecuteTransformer(lambda);
		}
	}
}