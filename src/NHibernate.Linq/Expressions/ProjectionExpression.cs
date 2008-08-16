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
		public ProjectionExpression(System.Type type,LambdaExpression lambda):base(SqlExpressionType.Projection,type)
		{
			this.Projector = lambda;
		}

		public LambdaExpression Projector { get; protected set; }
		public override string ToString()
		{
			return string.Format("({0})", this.Projector.ToString());
		}
	}
}
