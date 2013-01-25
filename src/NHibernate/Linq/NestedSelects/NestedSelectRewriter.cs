using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.NestedSelects
{
	static class NestedSelectRewriter
	{
		private static readonly MethodInfo CastMethod = EnumerableHelper.GetMethod("Cast", new[] { typeof (IEnumerable) }, new[] { typeof (object[]) });

		public static void ReWrite(QueryModel queryModel, ISessionFactory sessionFactory)
		{
			var nsqmv = new NestedSelectDetector();
			nsqmv.VisitExpression(queryModel.SelectClause.Selector);
			if (!nsqmv.HasSubquery)
				return;

			var subQueryExpression = GetSubQueryExpression(queryModel, nsqmv.Expression);
			if (subQueryExpression != null)
			{
				var subQueryModel = subQueryExpression.QueryModel;

				var mainFromClause = subQueryModel.MainFromClause;

				var restrictions = subQueryModel.BodyClauses
												.OfType<WhereClause>()
												.Select(w => new NhWithClause(w.Predicate));

				var join = new NhJoinClause(mainFromClause.ItemName,
											mainFromClause.ItemType,
											mainFromClause.FromExpression,
											restrictions);

				queryModel.BodyClauses.Add(join);

				var visitor = new SwapQuerySourceVisitor(subQueryModel.MainFromClause, join);

				queryModel.TransformExpressions(visitor.Swap);
			}

			var group = Expression.Parameter(typeof (IGrouping<Tuple, Tuple>), "g");

			var key = Expression.Property(group, "Key");

			var expressions = new List<ExpressionHolder>();

			var rewriter = new SelectClauseRewriter(key, group, expressions, GetIdentifier(sessionFactory, new QuerySourceReferenceExpression(queryModel.MainFromClause)));

			var resultSelector = rewriter.VisitExpression(queryModel.SelectClause.Selector);

			var keySelector = CreateSelector(expressions, 0);

			var elementSelector = CreateSelector(expressions, 1);

			var groupBy = EnumerableHelper.GetMethod("GroupBy",
													 new[] { typeof (IEnumerable<>), typeof (Func<,>), typeof (Func<,>) },
													 new[] { typeof (object[]), typeof (Tuple), typeof (Tuple) });

			var input = Expression.Parameter(typeof (IEnumerable<object>), "input");

			var lambda = Expression.Lambda(
				Expression.Call(groupBy,
								Expression.Call(CastMethod, input),
								keySelector,
								elementSelector),
				input);

			queryModel.ResultOperators.Add(new ClientSideSelect2(lambda));
			queryModel.ResultOperators.Add(new ClientSideSelect(Expression.Lambda(resultSelector, group)));

			var initializers = expressions.Select(e => e.Expression == null
														   ? GetIdentifier(sessionFactory, expressions, e)
														   : ConvertToObject(e.Expression));

			queryModel.SelectClause.Selector = Expression.NewArrayInit(typeof (object), initializers);
		}

		private static SubQueryExpression GetSubQueryExpression(QueryModel queryModel, Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression == null)
				return expression as SubQueryExpression;

			var mainFromClause = new MainFromClause(new NameGenerator(queryModel).GetNewName(),
													memberExpression.Type.GetGenericArguments()[0],
													memberExpression);
			var selectClause = new SelectClause(new QuerySourceReferenceExpression(mainFromClause));
			var subQueryModel = new QueryModel(mainFromClause, selectClause)
				{
					ResultTypeOverride = memberExpression.Type
				};
			var subQueryExpression = new SubQueryExpression(subQueryModel);
			queryModel.TransformExpressions(e => ReplacingExpressionTreeVisitor.Replace(memberExpression, subQueryExpression, e));
			return subQueryExpression;
		}

		private static Expression GetIdentifier(ISessionFactory sessionFactory, IEnumerable<ExpressionHolder> expressions, ExpressionHolder e)
		{
			foreach (var holder in expressions)
			{
				//TODO: move this code to SelectClauseRewriter
				if (holder.Tuple == e.Tuple)
				{
					//NOTE: probably will fail in some cases. Need to find underlying QuerySourceReferenceExpression and process it.
					var memberExpression = holder.Expression as MemberExpression;
					if (memberExpression != null)
					{
						return GetIdentifier(sessionFactory, memberExpression.Expression);
					}
					var querySourceReferenceExpression = holder.Expression as QuerySourceReferenceExpression;
					if (querySourceReferenceExpression != null)
					{
						return GetIdentifier(sessionFactory, querySourceReferenceExpression);
					}
				}
			}
			return Expression.Constant(null);
		}

		private static Expression GetIdentifier(ISessionFactory sessionFactory, Expression expression)
		{
			var classMetadata = sessionFactory.GetClassMetadata(expression.Type);
			if (classMetadata == null)
			return Expression.Constant(null);
			
			return ConvertToObject(Expression.PropertyOrField(expression, classMetadata.IdentifierPropertyName));
		}

		static LambdaExpression CreateSelector(IEnumerable<ExpressionHolder> expressions, int tuple)
		{
			var parameter = Expression.Parameter(typeof (object[]), "x");

			var initializers = expressions.Select((x, index) => new { x.Tuple, index})
				.Where(x => x.Tuple == tuple)
				.Select(x => ArrayIndex(x.index, parameter));

			var newArrayInit = Expression.NewArrayInit(typeof (object), initializers);

			return Expression.Lambda(
				Expression.MemberInit(
					Expression.New(typeof (Tuple)),
					Expression.Bind(Tuple.ItemsField, newArrayInit)),
				parameter);
		}

		static Expression ArrayIndex(int value, Expression param)
		{
			return Expression.ArrayIndex(param, Expression.Constant(value));
		}

		static Expression ConvertToObject(Expression expression)
		{
			return Expression.Convert(expression, typeof (object));
		}
	}
}