using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// The WhereJoinDetector creates the joins for the where clause, including
	/// optimizations for inner joins.
	/// 
	/// The detector asks the following question:
	/// Can an empty outer join ever return a record (ie. produce true in the where clause)?
	/// If not, it's equivalent to an inner join since empty joins that can't produce true
	/// never appear in the result set.
	/// 
	/// A record (object) will be in the result if the evaluation of the condition in 3-value SQL
	/// logic will return true; it will not be in the result if the result is either logical-null
	/// or false. The difference between outer joining and inner joining is that with the latter,
	/// objects are missing from the set on which the condition is checked. Thus, inner joins
	/// "emulates" a result that is logical-null or false. And therefore, we can replace an outer
	/// join with an inner join only if the resulting condition was not true on the outer join in
	/// the first place when there was an "empty outer join" - i.e., the outer join had to add
	/// nulls because there was no joinable record.  These nulls can appear even for a column
	/// that is not nullable.
	/// 
	/// For example:
	/// a.B.C == 1 could never produce true if B didn't match any rows, so it's safe to inner join.
	/// a.B.C == null could produce true even if B didn't match any rows, so we can't inner join.
	/// a.B.C == 1 && a.D.E == 1 can be inner joined.
	/// a.B.C == 1 || a.D.E == 1 must be outer joined.
	/// 
	/// By default we outer join via the code in VisitExpression.  The use of inner joins is only
	/// an optimization hint to the database.
	/// 
	/// More examples:
	/// a.B.C == 1 || a.B.C == null
	///     We don't need multiple joins for this.  When we reach the ||, we ask the value sets
	///     on either side if they have a value for when a.B.C is emptily outer joined.  Both of
	///     them do, so those values are combined.
	/// a.B.C == 1 || a.D.E == 1
	///     In this case, there is no value for a.B.C on the right side, so we use the possible
	///     values for the entire expression, ignoring specific members.  We only test for the
	///     empty outer joining of one member expression at a time, since we can't guarantee that
	///     they will all be emptily outer joined at the same time.
	/// a.B.C ?? a.D.E
	///     Even though each side is null when emptily outer joined, we can't promise that a.D.E
	///     will be emptily outer joined when a.B.C is.  Therefore, despite both sides being
	///     null, the result may not be.
	/// 
	/// There was significant discussion on the developers mailing list regarding this topic.  See also NH-2583.
	/// 
	/// The code here is based on the excellent work started by Harald Mueller.
	/// </summary>
	internal class WhereJoinDetector : ExpressionTreeVisitor
	{
		// TODO: There are a number of types of expressions that we didn't handle here due to time constraints.  For example, the ?: operator could be checked easily.
		private readonly IIsEntityDecider _isEntityDecider;
		private readonly IJoiner _joiner;

		private readonly Stack<bool> _handled = new Stack<bool>();
		
		// Stack of result values of each expression.  After an expression has processed itself, it adds itself to the stack.
		private readonly Stack<ExpressionValues> _values = new Stack<ExpressionValues>();

		// The following is used for member expressions traversal.
		private int _memberExpressionDepth;

		internal WhereJoinDetector(IIsEntityDecider isEntityDecider, IJoiner joiner)
		{
			_isEntityDecider = isEntityDecider;
			_joiner = joiner;
		}

		public void Transform(WhereClause whereClause)
		{
			whereClause.TransformExpressions(VisitExpression);

			var values = _values.Pop();

			foreach (var memberExpression in values.MemberExpressions)
			{
				// If outer join can never produce true, we can safely inner join.
				if (!values.GetValues(memberExpression).Contains(true))
				{
					_joiner.MakeInnerIfJoined(memberExpression);
				}
			}
		}

		public override Expression VisitExpression(Expression expression)
		{
			if (expression == null)
				return null;

			// To ensure safety in situations we don't understand, we default to "all possible values"
			// if we can't process any expression in a known way.  The SetResultValues() method is used
			// to indicate that the expression has been processed, and what the results are.

			_handled.Push(false);
			int originalCount = _values.Count;

			Expression result = base.VisitExpression(expression);

			if (!_handled.Pop())
			{
				// If this expression was not handled in a known way, we throw away any values that might
				// have been returned and we return "all values" for this expression, since we don't know
				// what the expresson might result in.
				while (_values.Count > originalCount)
					_values.Pop();
				_values.Push(new ExpressionValues(PossibleValueSet.CreateAllValues(expression.Type)));
			}

			return result;
		}

		protected override Expression VisitBinaryExpression(BinaryExpression expression)
		{
			var result = base.VisitBinaryExpression(expression);

			if (expression.NodeType == ExpressionType.AndAlso)
			{
				HandleBinaryOperation((a, b) => a.AndAlso(b));
			}
			else if (expression.NodeType == ExpressionType.OrElse)
			{
				HandleBinaryOperation((a, b) => a.OrElse(b));
			}
			else if (expression.NodeType == ExpressionType.NotEqual && VisitorUtil.IsNullConstant(expression.Right))
			{
				// Discard result from right null.  Left is visited first, so it's below right on the stack.
				_values.Pop();

				HandleUnaryOperation(pvs => pvs.IsNotNull());
			}
			else if (expression.NodeType == ExpressionType.NotEqual && VisitorUtil.IsNullConstant(expression.Left))
			{
				// Discard result from left null.
				var right = _values.Pop();
				_values.Pop(); // Discard left.
				_values.Push(right);

				HandleUnaryOperation(pvs => pvs.IsNotNull());
			}
			else if (expression.NodeType == ExpressionType.Equal && VisitorUtil.IsNullConstant(expression.Right))
			{
				// Discard result from right null.  Left is visited first, so it's below right on the stack.
				_values.Pop();

				HandleUnaryOperation(pvs => pvs.IsNull());
			}
			else if (expression.NodeType == ExpressionType.Equal && VisitorUtil.IsNullConstant(expression.Left))
			{
				// Discard result from left null.
				var right = _values.Pop();
				_values.Pop(); // Discard left.
				_values.Push(right);

				HandleUnaryOperation(pvs => pvs.IsNull());
			}
			else if (expression.NodeType == ExpressionType.Coalesce)
			{
				HandleBinaryOperation((a, b) => a.Coalesce(b));
			}
			else if (expression.NodeType == ExpressionType.Add || expression.NodeType == ExpressionType.AddChecked)
			{
				HandleBinaryOperation((a, b) => a.Add(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Divide)
			{
				HandleBinaryOperation((a, b) => a.Divide(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Modulo)
			{
				HandleBinaryOperation((a, b) => a.Modulo(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Multiply || expression.NodeType == ExpressionType.MultiplyChecked)
			{
				HandleBinaryOperation((a, b) => a.Multiply(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Power)
			{
				HandleBinaryOperation((a, b) => a.Power(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Subtract || expression.NodeType == ExpressionType.SubtractChecked)
			{
				HandleBinaryOperation((a, b) => a.Subtract(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.And)
			{
				HandleBinaryOperation((a, b) => a.And(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Or)
			{
				HandleBinaryOperation((a, b) => a.Or(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.ExclusiveOr)
			{
				HandleBinaryOperation((a, b) => a.ExclusiveOr(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.LeftShift)
			{
				HandleBinaryOperation((a, b) => a.LeftShift(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.RightShift)
			{
				HandleBinaryOperation((a, b) => a.RightShift(b, expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Equal)
			{
				HandleBinaryOperation((a, b) => a.Equal(b));
			}
			else if (expression.NodeType == ExpressionType.NotEqual)
			{
				HandleBinaryOperation((a, b) => a.NotEqual(b));
			}
			else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)
			{
				HandleBinaryOperation((a, b) => a.GreaterThanOrEqual(b));
			}
			else if (expression.NodeType == ExpressionType.GreaterThan)
			{
				HandleBinaryOperation((a, b) => a.GreaterThan(b));
			}
			else if (expression.NodeType == ExpressionType.LessThan)
			{
				HandleBinaryOperation((a, b) => a.LessThan(b));
			}
			else if (expression.NodeType == ExpressionType.LessThanOrEqual)
			{
				HandleBinaryOperation((a, b) => a.LessThanOrEqual(b));
			}

			return result;
		}

		protected override Expression VisitUnaryExpression(UnaryExpression expression)
		{
			Expression result = base.VisitUnaryExpression(expression);

			if (expression.NodeType == ExpressionType.Not && expression.Type == typeof(bool))
			{
				HandleUnaryOperation(pvs => pvs.Not());
			}
			else if (expression.NodeType == ExpressionType.Not)
			{
				HandleUnaryOperation(pvs => pvs.BitwiseNot(expression.Type));
			}
			else if (expression.NodeType == ExpressionType.ArrayLength)
			{
				HandleUnaryOperation(pvs => pvs.ArrayLength(expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
			{
				HandleUnaryOperation(pvs => pvs.Convert(expression.Type));
			}
			else if (expression.NodeType == ExpressionType.Negate || expression.NodeType == ExpressionType.NegateChecked)
			{
				HandleUnaryOperation(pvs => pvs.Negate(expression.Type));
			}
			else if (expression.NodeType == ExpressionType.UnaryPlus)
			{
				HandleUnaryOperation(pvs => pvs.UnaryPlus(expression.Type));
			}
			
			return result;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(VisitExpression);
			return expression;
		}

		// We would usually get NULL if one of our inner member expresions was null.
		// However, it's possible a method call will convert the null value from the failed join into a non-null value.
		// This could be optimized by actually checking what the method does.  For example StartsWith("s") would leave null as null and would still allow us to inner join.
		//protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
		//{
		//    Expression result = base.VisitMethodCallExpression(expression);
		//    return result;
		//}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			// The member expression we're visiting might be on the end of a variety of things, such as:
			//   a.B
			//   a.B.C
			//   (a.B ?? a.C).D
			// I'm not sure what processing re-linq does to strange member expressions.
			// TODO: I suspect this code doesn't add the right joins for the last case.

			var isIdentifier = _isEntityDecider.IsIdentifier(expression.Expression.Type, expression.Member.Name);

			if (!isIdentifier)
				_memberExpressionDepth++;

			var result = base.VisitMemberExpression(expression);

			if (!isIdentifier)
				_memberExpressionDepth--;

			ExpressionValues values = _values.Pop().Operation(pvs => pvs.MemberAccess(expression.Type));
			if (_isEntityDecider.IsEntity(expression.Type))
			{
				// Don't add joins for things like a.B == a.C where B and C are entities.
				// We only need to join B when there's something like a.B.D.
				var key = ExpressionKeyVisitor.Visit(expression, null);
				if (_memberExpressionDepth > 0 &&
					_joiner.CanAddJoin(expression))
				{
					result = _joiner.AddJoin(result, key);
				}

				values.MemberExpressionValuesIfEmptyOuterJoined[key] = PossibleValueSet.CreateNull(expression.Type);
			}
			SetResultValues(values);
			
			return result;
		}

		private void SetResultValues(ExpressionValues values)
		{
			_handled.Pop();
			_handled.Push(true);
			_values.Push(values);
		}

		private void HandleBinaryOperation(Func<PossibleValueSet, PossibleValueSet, PossibleValueSet> operation)
		{
			var right = _values.Pop();
			var left = _values.Pop();
			SetResultValues(left.Operation(right, operation));
		}

		private void HandleUnaryOperation(Func<PossibleValueSet, PossibleValueSet> operation)
		{
			SetResultValues(_values.Pop().Operation(operation));
		}

		private class ExpressionValues
		{
			public ExpressionValues(PossibleValueSet valuesIfUnknownMemberExpression)
			{
				Values = valuesIfUnknownMemberExpression;
				MemberExpressionValuesIfEmptyOuterJoined = new Dictionary<string, PossibleValueSet>();
			}

			/// <summary>
			/// Possible values of expression if there's set of values for the requested member expression.
			/// For example, if we have an expression "3" and we request the state for "a.B.C", we'll
			/// use "3" from Values since it won't exist in MemberExpressionValuesIfEmptyOuterJoined.
			/// </summary>
			private PossibleValueSet Values { get; set; }

			/// <summary>
			/// Stores the possible values of an expression that would result if the given member expression
			/// string was emptily outer joined.  For example a.B.C would result in "null" if we try to
			/// outer join to B and there are no rows.  Even if an expression tree does contain a particular
			/// member experssion, it may not appear in this list.  In that case, the emptily outer joined
			/// value set for that member expression will be whatever's in Values instead.
			/// </summary>
			public Dictionary<string, PossibleValueSet> MemberExpressionValuesIfEmptyOuterJoined { get; private set; }

			public PossibleValueSet GetValues(string memberExpression)
			{
				PossibleValueSet value;
				if (MemberExpressionValuesIfEmptyOuterJoined.TryGetValue(memberExpression, out value))
					return value;
				return Values;
			}

			public IEnumerable<string> MemberExpressions
			{
				get { return MemberExpressionValuesIfEmptyOuterJoined.Keys; }
			}

			public ExpressionValues Operation(ExpressionValues mergeWith, Func<PossibleValueSet, PossibleValueSet, PossibleValueSet> operation)
			{
				var result = new ExpressionValues(operation(Values, mergeWith.Values));
				foreach (string memberExpression in MemberExpressions.Union(mergeWith.MemberExpressions))
				{
					var left = GetValues(memberExpression);
					var right = mergeWith.GetValues(memberExpression);
					result.MemberExpressionValuesIfEmptyOuterJoined.Add(memberExpression, operation(left, right));
				}
				return result;
			}

			public ExpressionValues Operation(Func<PossibleValueSet, PossibleValueSet> operation)
			{
				var result = new ExpressionValues(operation(Values));
				foreach (string memberExpression in MemberExpressions)
				{
					result.MemberExpressionValuesIfEmptyOuterJoined.Add(memberExpression, operation(GetValues(memberExpression)));
				}
				return result;
			}
		}
	}
}
