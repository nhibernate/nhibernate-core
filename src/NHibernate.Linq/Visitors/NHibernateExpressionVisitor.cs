using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// A visitor for nhibernate specific expression
	/// </summary>
	public class NHibernateExpressionVisitor:ExpressionVisitor
	{
		public override Expression Visit(Expression exp)
		{
			if(exp==null)
				return null;
			switch((NHExpressionType)exp.NodeType)
			{
				case NHExpressionType.QuerySource:
					return this.VisitQuerySource((QuerySourceExpression)exp);
				case NHExpressionType.Select:
					return this.VisitSelect((SelectExpression)exp);
				case NHExpressionType.Projection:
					return this.VisitProjection((ProjectionExpression)exp);
				case NHExpressionType.Property:
					return this.VisitProperty((PropertyExpression)exp);
				default:
					return base.Visit(exp);
			}
		}

		protected virtual Expression VisitProjection(ProjectionExpression projection)
		{
			SelectExpression source = (SelectExpression)this.Visit(projection.Source);
			Expression projector = this.Visit(projection.Projector);
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
			Expression from = this.VisitSource(select.From);
			Expression where = this.Visit(select.Where);
			Expression projection = this.Visit(select.Projection);
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
