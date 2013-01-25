using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	class SelectClauseRewriter : ExpressionTreeVisitor
	{
		static readonly Expression<Func<Tuple, bool>> WherePredicate = t => !ReferenceEquals(null, t.Items[0]);

		readonly ICollection<ExpressionHolder> expressions;
		readonly Expression parameter;
		readonly ParameterExpression values;
		readonly int tuple;
		int position;

		public SelectClauseRewriter(Expression parameter, ParameterExpression values, ICollection<ExpressionHolder> expressions, Expression expression) 
			: this(parameter, values, expressions, expression, 0)
		{
		}

		public SelectClauseRewriter(Expression parameter, ParameterExpression values, ICollection<ExpressionHolder> expressions, Expression expression, int tuple)
		{
			this.expressions = expressions;
			this.parameter = parameter;
			this.values = values;
			this.tuple = tuple;
			this.expressions.Add(new ExpressionHolder { Expression = expression, Tuple = tuple }); //ID placeholder
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			return AddAndConvertExpression(expression);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			return AddAndConvertExpression(expression);
		}

		private Expression AddAndConvertExpression(Expression expression)
		{
			expressions.Add(new ExpressionHolder { Expression = expression, Tuple = tuple });

			return Expression.Convert(
				Expression.ArrayIndex(
					Expression.MakeMemberAccess(parameter,
												Tuple.ItemsField),
					Expression.Constant(++position)),
				expression.Type);
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			var selector = expression.QueryModel.SelectClause.Selector;

			var value = Expression.Parameter(typeof (Tuple), "value");

			var rewriter = new SelectClauseRewriter(value, values, expressions, null, tuple + 1);

			var resultSelector = rewriter.VisitExpression(selector);

			var whereMethod = EnumerableHelper.GetMethod("Where",
														 new[] { typeof (IEnumerable<>), typeof (Func<,>) },
														 new[] { typeof (Tuple) });

			var selectMethod = EnumerableHelper.GetMethod("Select",
														  new[] { typeof (IEnumerable<>), typeof (Func<,>) },
														  new[] { typeof (Tuple), selector.Type });

			var toListMethod = EnumerableHelper.GetMethod("ToList",
														  new[] { typeof (IEnumerable<>) },
														  new[] { selector.Type });

			var select = Expression.Call(selectMethod,
										 Expression.Call(whereMethod, values, WherePredicate),
										 Expression.Lambda(resultSelector, value));

			var constructor = GetCollectionConstructor(expression.QueryModel.GetResultType(), selector.Type);
			if (constructor != null)
				return Expression.New(constructor, @select);

			return Expression.Call(Expression.Call(toListMethod, @select),
								   "AsReadonly",
								   System.Type.EmptyTypes);
		}

		private static ConstructorInfo GetCollectionConstructor(System.Type collectionType, System.Type elementType)
		{
			if (collectionType.IsInterface)
			{
				if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof (ISet<>))
				{
					return typeof (HashSet<>).MakeGenericType(elementType).GetConstructor(new[] { typeof (IEnumerable<>).MakeGenericType(elementType) });
				}
				return null;
			}

			return collectionType.GetConstructor(new[] { typeof (IEnumerable<>).MakeGenericType(elementType) });
		}
	}
}