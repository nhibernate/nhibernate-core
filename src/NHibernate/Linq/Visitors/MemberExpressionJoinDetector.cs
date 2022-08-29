using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Detects joins in Select, OrderBy and Results (GroupBy) clauses.
	/// Replaces them with appropriate joins, maintaining reference equality between different clauses.
	/// This allows extracted GroupBy key expression to also be replaced so that they can continue to match replaced Select expressions
	/// </summary>
	internal class MemberExpressionJoinDetector : RelinqExpressionVisitor
	{
		private readonly IIsEntityDecider _isEntityDecider;
		private readonly IJoiner _joiner;
		private readonly ISessionFactoryImplementor _sessionFactory;

		private bool _requiresJoinForNonIdentifier;
		private bool _preventJoinsInConditionalTest;
		private bool _hasIdentifier;
		private int _memberExpressionDepth;

		public MemberExpressionJoinDetector(IIsEntityDecider isEntityDecider, IJoiner joiner, ISessionFactoryImplementor sessionFactory)
		{
			_isEntityDecider = isEntityDecider;
			_joiner = joiner;
			_sessionFactory = sessionFactory;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			// A static member expression such as DateTime.Now has a null Expression.
			if (expression.Expression == null)
			{
				// A static member call is never a join, and it is not an instance member access either.
				return base.VisitMember(expression);
			}

			var isEntity = _isEntityDecider.IsEntity(expression, out var isIdentifier);
			if (isIdentifier)
				_hasIdentifier = true;
			if (!isIdentifier)
				_memberExpressionDepth++;

			var result = base.VisitMember(expression);

			if (!isIdentifier)
				_memberExpressionDepth--;

			if (isEntity &&
				((_requiresJoinForNonIdentifier && !_hasIdentifier) || _memberExpressionDepth > 0) &&
				_joiner.CanAddJoin(expression))
			{
				var key = ExpressionKeyVisitor.Visit(expression, null, _sessionFactory);
				return _joiner.AddJoin(result, key);
			}

			_hasIdentifier = false;
			return result;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			_memberExpressionDepth++;
			var result = base.VisitMethodCall(node);
			_memberExpressionDepth--;
			return result;
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(Visit);
			return expression;
		}

		protected override Expression VisitConditional(ConditionalExpression expression)
		{
			var oldRequiresJoinForNonIdentifier = _requiresJoinForNonIdentifier;
			_requiresJoinForNonIdentifier = !_preventJoinsInConditionalTest && _requiresJoinForNonIdentifier;
			var newTest = Visit(expression.Test);
			_requiresJoinForNonIdentifier = oldRequiresJoinForNonIdentifier;
			var newFalse = Visit(expression.IfFalse);
			var newTrue = Visit(expression.IfTrue);
			return expression.Update(newTest, newTrue, newFalse);
		}

		protected override Expression VisitExtension(Expression expression)
		{
			// Nominated expressions need to prevent joins on non-Identifier member expressions 
			// (for the test expression of conditional expressions only)
			// Otherwise an extra join is created and the GroupBy and Select clauses will not match
			var old = _preventJoinsInConditionalTest;
			_preventJoinsInConditionalTest = expression is NhNominatedExpression;
			var expr = base.VisitExtension(expression);
			_preventJoinsInConditionalTest = old;
			return expr;
		}

		public void Transform(SelectClause selectClause)
		{
			// The select clause typically requires joins for non-Identifier member access
			_requiresJoinForNonIdentifier = true;
			selectClause.TransformExpressions(Visit);
			_requiresJoinForNonIdentifier = false;
		}

		public void Transform(ResultOperatorBase resultOperator)
		{
			resultOperator.TransformExpressions(Visit);
		}

		public void Transform(Ordering ordering)
		{
			ordering.TransformExpressions(Visit);
		}
	}
}
