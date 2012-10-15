using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	class SelectClauseRewriter : ExpressionTreeVisitor
	{
		static readonly Expression<Func<Tuple, bool>> WherePredicate = t => !ReferenceEquals(null, t.Items[0]);

		readonly ICollection<ExpressionHolder> expressions;
		readonly ParameterExpression parameter;
		readonly ParameterExpression values;
		readonly int tuple;
		int position;

		public SelectClauseRewriter(ParameterExpression parameter, ParameterExpression values, ICollection<ExpressionHolder> expressions) 
			: this(parameter, values, expressions, 0)
		{
		}

		public SelectClauseRewriter(ParameterExpression parameter, ParameterExpression values, ICollection<ExpressionHolder> expressions, int tuple)
		{
			this.expressions = expressions;
			this.parameter = parameter;
			this.values = values;
			this.tuple = tuple;
			this.expressions.Add(new ExpressionHolder {Tuple = tuple}); //ID placeholder
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			expressions.Add(new ExpressionHolder {Expression = expression, Tuple = tuple});

			return Expression.Convert(
				Expression.ArrayIndex(
					Expression.MakeMemberAccess(parameter,
												Tuple.Type.GetField("Items")),
					Expression.Constant(++position)),
				expression.Type);
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			var selector = expression.QueryModel.SelectClause.Selector;

			var value = Expression.Parameter(typeof (Tuple), "value");

			var rewriter = new SelectClauseRewriter(value, values, expressions, tuple + 1);

			var resultSelector = rewriter.VisitExpression(selector);

			var where = EnumerableHelper.GetMethod("Where",
												   new[] {typeof (IEnumerable<>), typeof (Func<,>)},
												   new[] {Tuple.Type});

			var select = EnumerableHelper.GetMethod("Select",
													new[] {typeof (IEnumerable<>), typeof (Func<,>)},
													new[] {Tuple.Type, selector.Type});

			var toList = EnumerableHelper.GetMethod("ToList",
													new[] {typeof (IEnumerable<>)},
													new[] {selector.Type});

			return Expression.Call(Expression.Call(toList,
												   Expression.Call(select,
																   Expression.Call(where, values, WherePredicate),
																   Expression.Lambda(resultSelector, value))),
								   "AsReadOnly", System.Type.EmptyTypes);
		}
	}
}