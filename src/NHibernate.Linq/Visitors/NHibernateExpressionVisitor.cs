using System.Linq.Expressions;
using NHibernate.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// A visitor for nhibernate specific expression
	/// </summary>
	public class NHibernateExpressionVisitor : ExpressionVisitor
	{
		public override Expression Visit(Expression exp)
		{
			if (exp == null)
				return null;
			switch ((NHExpressionType) exp.NodeType)
			{
				case NHExpressionType.QuerySource:
					return VisitQuerySource((QuerySourceExpression) exp);
				case NHExpressionType.Select:
					return VisitSelect((SelectExpression) exp);
				case NHExpressionType.Projection:
					return VisitProjection((ProjectionExpression) exp);
				case NHExpressionType.Property:
					return VisitProperty((PropertyExpression) exp);
				default:
					return base.Visit(exp);
			}
		}

		protected virtual Expression VisitProjection(ProjectionExpression projection)
		{
			var source = (SelectExpression) Visit(projection.Source);
			Expression projector = Visit(projection.Projector);
			if (source != projection.Source || projector != projection.Projector)
			{
				return new ProjectionExpression(source, projector);
			}
			else
				return projection;
		}

		protected virtual Expression VisitSource(Expression source)
		{
			return source;
		}

		protected virtual Expression VisitProperty(PropertyExpression property)
		{
			return property;
		}

		//TODO: modify
		protected virtual Expression VisitSelect(SelectExpression select)
		{
			Expression from = VisitSource(select.From);
			Expression where = Visit(select.Where);
			Expression projection = Visit(select.Projection);
			if (from != select.From || where != select.Where)
			{
				return new SelectExpression(select.Type, select.FromAlias, projection, from, where);
			}
			else
				return select;
		}

		protected virtual Expression VisitQuerySource(QuerySourceExpression expr)
		{
			return expr;
		}
	}
}