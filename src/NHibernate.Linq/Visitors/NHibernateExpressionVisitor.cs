using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using System.Linq;
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
				case NHExpressionType.SimpleProperty:
					return VisitSimpleProperty((SimplePropertyExpression) exp);
				case NHExpressionType.CollectionProperty:
					return VisitCollectionProperty((CollectionPropertyExpression)exp);
				case NHExpressionType.ComponentProperty:
					return VisitComponentProperty((ComponentPropertyExpression) exp);
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

		protected virtual Expression VisitSimpleProperty(SimplePropertyExpression property)
		{
			Expression source = Visit(property.Source);
			if (source != property.Source)
				return new SimplePropertyExpression(property.Name, property.Column, property.Type, source, property.NHibernateType);
			return property;
		}
		private Expression VisitComponentProperty(ComponentPropertyExpression property)
		{
			Expression source = Visit(property.Source);
			if (source != property.Source)
				return new ComponentPropertyExpression(property.Name, property.Columns, property.Type, source, property.NHibernateType);
			return property;
		}

		private Expression VisitCollectionProperty(CollectionPropertyExpression property)
		{
			Expression source = Visit(property.Source);
			if (source != property.Source)
				return new CollectionPropertyExpression(property.Name, property.Type, source, property.NHibernateType);
			return property;
		}

		//TODO: modify
		protected virtual Expression VisitSelect(SelectExpression select)
		{
			Expression from = VisitSource(select.From);
			Expression where = Visit(select.Where);
			Expression projection = Visit(select.Projection);
			ReadOnlyCollection<Expression> orderbys = VisitList(select.OrderBys);
			if (from != select.From || where != select.Where)
			{
				return new SelectExpression(select.Type, select.FromAlias, projection, from, where, orderbys);
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