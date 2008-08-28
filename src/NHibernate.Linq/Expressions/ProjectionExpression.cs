using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	public class ProjectionExpression:NHExpression
	{
		public ProjectionExpression(SelectExpression source, Expression projector)
			: base(NHExpressionType.Projection, projector.Type)
		{
			this.Projector = projector;
			this.Source = source;
		}

		public Expression Projector { get; protected set; }
		public SelectExpression Source { get; protected set; }
		public override string ToString()
		{
			return string.Format("({0})", this.Projector);
		}
	}
}
