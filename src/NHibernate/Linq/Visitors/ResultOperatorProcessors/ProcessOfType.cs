using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessOfType : IResultOperatorProcessor<OfTypeResultOperator>
	{
		#region IResultOperatorProcessor<OfTypeResultOperator> Members

		public void Process(OfTypeResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
		    Expression source = queryModelVisitor.Model.SelectClause.GetOutputDataInfo().ItemExpression;

			tree.AddWhereClause(tree.TreeBuilder.Equality(
				tree.TreeBuilder.Dot(
					HqlGeneratorExpressionTreeVisitor.Visit(source, queryModelVisitor.VisitorParameters).AsExpression(),
					tree.TreeBuilder.Class()),
				tree.TreeBuilder.Ident(resultOperator.SearchedItemType.FullName)));
		}

		#endregion
	}
}