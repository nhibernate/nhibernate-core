using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	class SelectClauseRewriter : NhExpressionTreeVisitor
	{
		public readonly ICollection<ExpressionHolder> expressions = new List<ExpressionHolder>();
		readonly ParameterExpression keyTuple;
		readonly ParameterExpression value = Expression.Parameter(typeof (Tuple), "value");
		readonly ParameterExpression values;
		readonly IDictionary<int, int> positions = new Dictionary<int, int>();
		int tuple;

		public SelectClauseRewriter(ParameterExpression keyTuple, ParameterExpression values)
		{
			this.keyTuple = keyTuple;
			this.values = values;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			int position;
			positions.TryGetValue(tuple, out position);
			positions[tuple] = position + 1;

			expressions.Add(new ExpressionHolder {Expression = expression, Tuple = tuple});

			return Expression.Convert(
				Expression.ArrayIndex(
					Expression.MakeMemberAccess(tuple == 0 ? keyTuple : value,
												Tuple.Type.GetField("Items")),
					Expression.Constant(position)),
				expression.Type);
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			var selector = expression.QueryModel.SelectClause.Selector;
			tuple++;
			var visitExpression = VisitExpression(selector);
			tuple--;

			var select = EnumerableHelper.GetMethod("Select",
													new[] {typeof (IEnumerable<>), typeof (Func<,>)},
													new[] {Tuple.Type, selector.Type});

			var toList = EnumerableHelper.GetMethod("ToList",
													new[] {typeof (IEnumerable<>)},
													new[] {selector.Type});

			return Expression.Call(Expression.Call(toList,
												   Expression.Call(select,
																   values,
																   Expression.Lambda(visitExpression, value))),
								   "AsReadOnly", System.Type.EmptyTypes);
		}
	}
}