using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
	public class SelectAndOrderByJoinDetector : NhExpressionTreeVisitor
	{
		private readonly IIsEntityDecider _isEntityDecider;
		private readonly IJoiner _joiner;

		internal SelectAndOrderByJoinDetector(IIsEntityDecider isEntityDecider, IJoiner joiner)
		{
			_isEntityDecider = isEntityDecider;
			_joiner = joiner;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			var result = base.VisitMemberExpression(expression);

			if (expression.Type.IsNonPrimitive() && _isEntityDecider.IsEntity(expression.Type))
			{
				var key = ExpressionKeyVisitor.Visit(expression, null);
				return _joiner.AddJoin(result, key);
			}

			return result;
		}

		public void Transform(SelectClause selectClause)
		{
			selectClause.TransformExpressions(VisitExpression);
		}

		public void Transform(Ordering ordering)
		{
			ordering.TransformExpressions(VisitExpression);
}
	}
}
