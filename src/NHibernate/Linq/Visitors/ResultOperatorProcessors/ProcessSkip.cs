using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Param;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessSkip : IResultOperatorProcessor<SkipResultOperator>
	{
		#region IResultOperatorProcessor<SkipResultOperator> Members

		public void Process(SkipResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			VisitorParameters parameters = queryModelVisitor.VisitorParameters;
			NamedParameter namedParameter;

			if (parameters.ConstantToParameterMap.TryGetValue(resultOperator.Count as ConstantExpression, out namedParameter))
			{
				parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, false));
				tree.AddSkipClause(tree.TreeBuilder.Parameter(namedParameter.Name));
			}
			else
			{
				tree.AddSkipClause(tree.TreeBuilder.Constant(resultOperator.GetConstantCount()));
			}
		}

		#endregion
	}
}