using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.GroupBy;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessClientSideSelect : IResultOperatorProcessor<ClientSideSelect>
	{
		private static readonly MethodInfo SelectMethodDefinition = ReflectionHelper.GetMethodDefinition(
			() => Enumerable.Select<object, object>(null, (Func<object, object>)null));
		private static readonly MethodInfo ToListMethodDefinition = ReflectionHelper.GetMethodDefinition(
			() => Enumerable.ToList<object>(null));

		public void Process(ClientSideSelect resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var inputType = resultOperator.SelectClause.Parameters[0].Type;
			var outputType = resultOperator.SelectClause.Type.GetGenericArguments()[1];

			var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(inputType), "inputList");

			var selectMethod = SelectMethodDefinition.MakeGenericMethod(new[] { inputType, outputType });
			var toListMethod = ToListMethodDefinition.MakeGenericMethod(new[] { outputType });

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