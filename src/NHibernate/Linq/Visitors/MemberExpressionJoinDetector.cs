using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
	internal class MemberExpressionJoinDetector : ExpressionTreeVisitor
	{
		private readonly IIsEntityDecider _isEntityDecider;
		private readonly IJoiner _joiner;

		private bool _requiresJoinForNonIdentifier;
		private bool _preventJoinsInConditionalTest;
		private bool _hasIdentifier;
		private int _memberExpressionDepth;

		public MemberExpressionJoinDetector(IIsEntityDecider isEntityDecider, IJoiner joiner)
		{
			_isEntityDecider = isEntityDecider;
			_joiner = joiner;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			var isIdentifier = _isEntityDecider.IsIdentifier(expression.Expression.Type, expression.Member.Name);
			if (isIdentifier)
				_hasIdentifier = true;
			if (!isIdentifier)
				_memberExpressionDepth++;

			var result = base.VisitMemberExpression(expression);
			
			if (!isIdentifier)
				_memberExpressionDepth--;

			if (_isEntityDecider.IsEntity(expression.Type) &&
				((_requiresJoinForNonIdentifier && !_hasIdentifier) || _memberExpressionDepth > 0) &&
				_joiner.CanAddJoin(expression))
			{
				var key = ExpressionKeyVisitor.Visit(expression, null);
				return _joiner.AddJoin(result, key);
			}

			return result;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(VisitExpression);
			return expression;
		}

		protected override Expression VisitConditionalExpression(ConditionalExpression expression)
		{
			var oldRequiresJoinForNonIdentifier = _requiresJoinForNonIdentifier;
			_requiresJoinForNonIdentifier = !_preventJoinsInConditionalTest && _requiresJoinForNonIdentifier;
			var newTest = VisitExpression(expression.Test);
			_requiresJoinForNonIdentifier = oldRequiresJoinForNonIdentifier;
			var newFalse = VisitExpression(expression.IfFalse);
			var newTrue = VisitExpression(expression.IfTrue);
			if ((newTest != expression.Test) || (newFalse != expression.IfFalse) || (newTrue != expression.IfTrue))
				return Expression.Condition(newTest, newTrue, newFalse);
			return expression;
		}

		protected override Expression VisitExtensionExpression(ExtensionExpression expression)
		{
			// Nominated expressions need to prevent joins on non-Identifier member expressions 
			// (for the test expression of conditional expressions only)
			// Otherwise an extra join is created and the GroupBy and Select clauses will not match
			var old = _preventJoinsInConditionalTest;
			_preventJoinsInConditionalTest = (NhExpressionType)expression.NodeType == NhExpressionType.Nominator;
			var expr = base.VisitExtensionExpression(expression);
			_preventJoinsInConditionalTest = old;
			return expr;
		}

		public void Transform(SelectClause selectClause)
		{
			// The select clause typically requires joins for non-Identifier member access
			_requiresJoinForNonIdentifier = true;
			selectClause.TransformExpressions(VisitExpression);
			_requiresJoinForNonIdentifier = false;
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