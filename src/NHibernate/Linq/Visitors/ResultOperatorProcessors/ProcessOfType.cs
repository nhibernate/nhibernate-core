using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessOfType : IResultOperatorProcessor<OfTypeResultOperator>
	{
		public void Process(OfTypeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			var source = queryModelVisitor.Model.SelectClause.GetOutputDataInfo().ItemExpression;

			var expression = new HqlGeneratorExpressionTreeVisitor(queryModelVisitor.VisitorParameters)
				.BuildOfType(source, resultOperator.SearchedItemType);

			tree.AddWhereClause(expression);
		}
	}
}