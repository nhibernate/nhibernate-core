using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.GroupBy;
using NHibernate.Util;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessClientSideSelect : IResultOperatorProcessor<ClientSideSelect>
	{
		public void Process(ClientSideSelect resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var inputType = resultOperator.SelectClause.Parameters[0].Type;
			var outputType = resultOperator.SelectClause.Type.GetGenericArguments()[1];

			var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(inputType), "inputList");

			var selectMethod = ReflectionCache.EnumerableMethods.SelectDefinition.MakeGenericMethod(new[] { inputType, outputType });
			var toListMethod = ReflectionCache.EnumerableMethods.ToListDefinition.MakeGenericMethod(new[] { outputType });

			var lambda = Expression.Lambda(
				Expression.Call(toListMethod,
								Expression.Call(selectMethod, inputList, resultOperator.SelectClause)),
				inputList);

			tree.AddListTransformer(lambda);
		}
	}
	public class ProcessClientSideSelect2 : IResultOperatorProcessor<ClientSideSelect2>
	{
		public void Process(ClientSideSelect2 resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddListTransformer(resultOperator.SelectClause);
		}
	}
}