using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
	public class SelectAndOrderByJoinDetector : NhExpressionTreeVisitor
	{
		private readonly IIsEntityDecider _isEntityDecider;
		private readonly IJoiner _joiner;
		private bool _isIdentifier;

		internal SelectAndOrderByJoinDetector(IIsEntityDecider isEntityDecider, IJoiner joiner)
		{
			_isEntityDecider = isEntityDecider;
			_joiner = joiner;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			_isIdentifier |= _isEntityDecider.IsIdentifier(expression.Expression.Type, expression.Member.Name);

			var result = base.VisitMemberExpression(expression);

			if (expression.Type.IsNonPrimitive() && _isEntityDecider.IsEntity(expression.Type) && !_isIdentifier)
			{
				var key = ExpressionKeyVisitor.Visit(expression, null);
				return _joiner.AddJoin(result, key);
			}

			_isIdentifier = false;
			
			return result;
		}

		public void Transform(SelectClause selectClause)
		{
			selectClause.TransformExpressions(VisitExpression);
		}

		public void Transform(ResultOperatorBase resultOperator)
		{
			resultOperator.TransformExpressions(VisitExpression);
		}

		public void Transform(Ordering ordering)
		{
			ordering.TransformExpressions(VisitExpression);
		}
	}
}
