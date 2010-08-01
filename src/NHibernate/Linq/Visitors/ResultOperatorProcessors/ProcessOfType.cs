using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessOfType : IResultOperatorProcessor<OfTypeResultOperator>
	{
		#region IResultOperatorProcessor<OfTypeResultOperator> Members

		public void Process(OfTypeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			Expression source =
				queryModelVisitor.CurrentEvaluationType.As<StreamedSequenceInfo>().ItemExpression;

			tree.AddWhereClause(tree.TreeBuilder.Equality(
				tree.TreeBuilder.Dot(
					HqlGeneratorExpressionTreeVisitor.Visit(source, queryModelVisitor.VisitorParameters).AsExpression(),
					tree.TreeBuilder.Class()),
				tree.TreeBuilder.Ident(resultOperator.SearchedItemType.FullName)));
		}

		#endregion
	}
}