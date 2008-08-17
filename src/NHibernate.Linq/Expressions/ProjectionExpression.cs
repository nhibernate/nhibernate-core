using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// Projection expression
	/// </summary>
	public class ProjectionExpression:SqlExpression
	{
		public ProjectionExpression(SelectExpression source, Expression lambda)
			: base(SqlExpressionType.Projection, lambda.Type)
		{
			this.Projector = lambda;
		}

		public Expression Projector { get; protected set; }
		public SelectExpression Source { get; set; }
		public override string ToString()
		{
			return string.Format("({0})", this.Projector.ToString());
		}
	}
}
