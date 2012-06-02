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

namespace NHibernate.Linq.NestedSelects
{
	static class NestedSelectRewriter
	{
		public static void ReWrite(QueryModel queryModel, ISessionFactory sessionFactory)
		{
			var nsqmv = new NestedSelectDetector();
			nsqmv.VisitExpression(queryModel.SelectClause.Selector);
			if (!nsqmv.HasSubquery)
				return;

			var subQueryModel = nsqmv.Expression.QueryModel;

			var mainFromClause = subQueryModel.MainFromClause;

			var item = new NhJoinClause(mainFromClause.ItemName, mainFromClause.ItemType, mainFromClause.FromExpression);

			queryModel.BodyClauses.Add(item);

			var visitor = new SwapQuerySourceVisitor(subQueryModel.MainFromClause, item);

			queryModel.TransformExpressions(visitor.Swap);

			var ctor = Tuple.Type.GetConstructor(System.Type.EmptyTypes);
			
			var key = Expression.Parameter(Tuple.Type, "key");

			var values = Expression.Parameter(typeof (IEnumerable<Tuple>), "values");

			var expressions = new List<ExpressionHolder>();

			var rewriter = new SelectClauseRewriter(key, values, expressions);

			var resultSelector = rewriter.VisitExpression(queryModel.SelectClause.Selector);

			var field = Tuple.Type.GetField("Items");

			var keySelector = CreateSelector(ctor, field, expressions, 0);

			var elementSelector = CreateSelector(ctor, field, expressions, 1);

			var cast = EnumerableHelper.GetMethod("Cast", new[] {typeof (IEnumerable)}, new[] {typeof (object[])});

			var groupBy = EnumerableHelper.GetMethod("GroupBy",
													 new[] {typeof (IEnumerable<>), typeof (Func<,>), typeof (Func<,>), typeof (Func<,,>)},
													 new[] {typeof (object[]), Tuple.Type, Tuple.Type, queryModel.SelectClause.Selector.Type});

			var toList = EnumerableHelper.GetMethod("ToList", new[] { typeof(IEnumerable<>) }, new[] { queryModel.SelectClause.Selector.Type });
			
			var input = Expression.Parameter(typeof (IEnumerable<object>), "input");

			var call = Expression.Call(toList,
									   Expression.Call(groupBy,
													   Expression.Call(cast, input),
													   keySelector,
													   elementSelector,
													   Expression.Lambda(resultSelector, key, values)));

			var lambda = Expression.Lambda(call, input);

			queryModel.ResultOperators.Add(new ClientSideSelect2(lambda));

			var initializers = expressions.Select(e => e.Expression == null
														   ? GetIdentifier(sessionFactory, expressions, e)
														   : ConvertToObject(e.Expression));

			queryModel.SelectClause.Selector = Expression.NewArrayInit(typeof (object), initializers);
		}

		static Expression GetIdentifier(ISessionFactory sessionFactory, IEnumerable<ExpressionHolder> expressions, ExpressionHolder e)
		{
			foreach (var holders in expressions)
			{
				if (holders.Tuple == e.Tuple)
				{
					var memberExpression = holders.Expression as MemberExpression;
					if (memberExpression != null)
					{
						var expression = memberExpression.Expression;
						var classMetadata = sessionFactory.GetClassMetadata(expression.Type);
						if (classMetadata != null)
						{
							return ConvertToObject(Expression.PropertyOrField(expression, classMetadata.IdentifierPropertyName));
						}
					}
				}
			}
			return Expression.Constant(null);
		}

		static LambdaExpression CreateSelector(ConstructorInfo ctor, MemberInfo field, IEnumerable<ExpressionHolder> expressions, int tuple)
		{
			var parameter = Expression.Parameter(typeof (object[]), "x");

			var initializers = expressions.Select((x, index) => new {x.Tuple, index})
				.Where(x => x.Tuple == tuple)
				.Select(x => ArrayIndex(x.index, parameter));

			var newArrayInit = Expression.NewArrayInit(typeof (object), initializers);
			
			return Expression.Lambda(
				Expression.MemberInit(
					Expression.New(ctor),
					Expression.Bind(field, newArrayInit)),
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