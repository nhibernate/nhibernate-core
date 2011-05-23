using System.Linq.Expressions;
using NHibernate.Engine.Query;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessTake : IResultOperatorProcessor<TakeResultOperator>
	{
		#region IResultOperatorProcessor<TakeResultOperator> Members

		public void Process(TakeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			VisitorParameters parameters = queryModelVisitor.VisitorParameters;
			NamedParameter namedParameter;

			if (parameters.ConstantToParameterMap.TryGetValue(resultOperator.Count as ConstantExpression, out namedParameter))
			{
				parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, new[] {parameters.RequiredHqlParameters.Count + 1}, false));
				tree.AddTakeClause(tree.TreeBuilder.Parameter(namedParameter.Name));
			}
			else
			{
				tree.AddTakeClause(tree.TreeBuilder.Constant(resultOperator.GetConstantCount()));
			}
		}

		#endregion
	}
}