using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
	internal class ResultOperatorAndOrderByJoinDetector : NhExpressionTreeVisitor
	{
		private readonly IIsEntityDecider _isEntityDecider;
		private readonly IJoiner _joiner;
		private int _memberExpressionDepth;

		public ResultOperatorAndOrderByJoinDetector(IIsEntityDecider isEntityDecider, IJoiner joiner)
		{
			_isEntityDecider = isEntityDecider;
			_joiner = joiner;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			var isIdentifier = _isEntityDecider.IsIdentifier(expression.Expression.Type, expression.Member.Name);
			if (!isIdentifier)
				_memberExpressionDepth++;

			var result = base.VisitMemberExpression(expression);
			
			if (!isIdentifier)
				_memberExpressionDepth--;

			if (_isEntityDecider.IsEntity(expression.Type) && _memberExpressionDepth > 0)
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