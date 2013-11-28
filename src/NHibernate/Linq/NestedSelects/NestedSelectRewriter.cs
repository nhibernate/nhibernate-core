using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	static class NestedSelectRewriter
	{
		private static readonly MethodInfo CastMethod = EnumerableHelper.GetMethod("Cast", new[] { typeof (IEnumerable) }, new[] { typeof (object[]) });

		public static void ReWrite(QueryModel queryModel, ISessionFactory sessionFactory)
		{
			var nsqmv = new NestedSelectDetector(sessionFactory);
			nsqmv.VisitExpression(queryModel.SelectClause.Selector);
			if (!nsqmv.HasSubqueries)
				return;

			var elementExpression = new List<ExpressionHolder>();
			var group = Expression.Parameter(typeof (IGrouping<Tuple, Tuple>), "g");
			
			var replacements = new Dictionary<Expression, Expression>();
			foreach (var expression in nsqmv.Expressions)
			{
				var processed = ProcessExpression(queryModel, sessionFactory, expression, elementExpression, group);
				if (processed != null)
					replacements.Add(expression, processed);
			}

			var key = Expression.Property(group, "Key");

			var expressions = new List<ExpressionHolder>();

			var identifier = GetIdentifier(sessionFactory, new QuerySourceReferenceExpression(queryModel.MainFromClause));

			var rewriter = new SelectClauseRewriter(key, expressions, identifier, replacements);

			var resultSelector = rewriter.VisitExpression(queryModel.SelectClause.Selector);

			elementExpression.AddRange(expressions);

			var keySelector = CreateSelector(elementExpression, 0);

			var elementSelector = CreateSelector(elementExpression, 1);

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
			queryModel.ResultOperators.Add(new ClientSideSelect(Expression.Lambda(resultSelector, @group)));

			var initializers = elementExpression.Select(e => ConvertToObject(e.Expression));

			queryModel.SelectClause.Selector = Expression.NewArrayInit(typeof (object), initializers);
		}

		private static Expression ProcessExpression(QueryModel queryModel, ISessionFactory sessionFactory, Expression expression, List<ExpressionHolder> elementExpression, ParameterExpression @group)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
				return ProcessMemberExpression(sessionFactory, elementExpression, queryModel, @group, memberExpression);
			
			var subQueryExpression = expression as SubQueryExpression;
			if (subQueryExpression != null)
				return ProcessSubquery(sessionFactory, elementExpression, queryModel, @group, subQueryExpression.QueryModel);
			
			return null;
		}

		private static Expression ProcessSubquery(ISessionFactory sessionFactory, ICollection<ExpressionHolder> elementExpression, QueryModel queryModel, Expression @group, QueryModel subQueryModel)
		{
			var subQueryMainFromClause = subQueryModel.MainFromClause;

			var restrictions = subQueryModel.BodyClauses
											.OfType<WhereClause>()
											.Select(w => new NhWithClause(w.Predicate));

			var join = new NhJoinClause(subQueryMainFromClause.ItemName,
										subQueryMainFromClause.ItemType,
										subQueryMainFromClause.FromExpression,
										restrictions);

			queryModel.BodyClauses.Add(@join);

			var visitor = new SwapQuerySourceVisitor(subQueryMainFromClause, @join);

			queryModel.TransformExpressions(visitor.Swap);

			var selector = subQueryModel.SelectClause.Selector;

			var collectionType = subQueryModel.GetResultType();
			
			var elementType = selector.Type;

			var source = new QuerySourceReferenceExpression(@join);

			return BuildSubCollectionQuery(sessionFactory, elementExpression, @group, source, selector, elementType, collectionType);
		}

		private static Expression ProcessMemberExpression(ISessionFactory sessionFactory, ICollection<ExpressionHolder> elementExpression, QueryModel queryModel, Expression @group, Expression memberExpression)
		{
			var join = new NhJoinClause(new NameGenerator(queryModel).GetNewName(),
										GetElementType(memberExpression.Type),
										memberExpression);

			queryModel.BodyClauses.Add(@join);

			var source = new QuerySourceReferenceExpression(@join);

			return BuildSubCollectionQuery(sessionFactory, elementExpression, @group, source, source, source.Type, memberExpression.Type);
		}

		private static Expression BuildSubCollectionQuery(ISessionFactory sessionFactory, ICollection<ExpressionHolder> expressions, Expression @group, Expression source, Expression select, System.Type elementType, System.Type collectionType)
		{
			var predicate = MakePredicate(expressions.Count);

			var identifier = GetIdentifier(sessionFactory, source);

			var selector = MakeSelector(expressions, @select, identifier);

			return SubCollectionQuery(collectionType, elementType, @group, predicate, selector);
		}

		private static LambdaExpression MakeSelector(ICollection<ExpressionHolder> elementExpression, Expression @select, Expression identifier)
		{
			var parameter = Expression.Parameter(typeof (Tuple), "value");

			var rewriter = new SelectClauseRewriter(parameter, elementExpression, identifier, 1, new Dictionary<Expression, Expression>());

			var selectorBody = rewriter.VisitExpression(@select);

			return Expression.Lambda(selectorBody, parameter);
		}

		private static Expression SubCollectionQuery(System.Type collectionType, System.Type elementType, Expression source, Expression predicate, Expression selector)
		{
			// source.Where(predicate).Select(selector).ToList();
			var whereMethod = EnumerableHelper.GetMethod("Where",
														  new[] { typeof (IEnumerable<>), typeof (Func<,>) },
														  new[] { typeof (Tuple) });


			var selectMethod = EnumerableHelper.GetMethod("Select",
														   new[] { typeof (IEnumerable<>), typeof (Func<,>) },
														   new[] { typeof (Tuple), elementType });

			var select = Expression.Call(selectMethod,
										 Expression.Call(whereMethod, source, predicate),
										 selector);

			if (collectionType.IsArray)
			{
				var toArrayMethod = EnumerableHelper.GetMethod("ToArray",
															  new[] { typeof(IEnumerable<>) },
															  new[] { elementType });

				var array = Expression.Call(toArrayMethod, @select);
				return array;
			}

			var constructor = GetCollectionConstructor(collectionType, elementType);
			if (constructor != null)
				return Expression.New(constructor, (Expression) @select);

			var toListMethod = EnumerableHelper.GetMethod("ToList",
														  new[] { typeof (IEnumerable<>) },
														  new[] { elementType });

			return Expression.Call(Expression.Call(toListMethod, @select),
								   "AsReadonly",
								   System.Type.EmptyTypes);
		}

		private static ConstructorInfo GetCollectionConstructor(System.Type collectionType, System.Type elementType)
		{
			if (collectionType.IsInterface)
			{
				if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(ISet<>))
				{
					return typeof(HashSet<>).MakeGenericType(elementType).GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
				}
				return null;
			}

			return collectionType.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
		}

		private static LambdaExpression MakePredicate(int index)
		{
			// t => Not(ReferenceEquals(t.Items[index], null))
			var t = Expression.Parameter(typeof (Tuple), "t");
			return Expression.Lambda(
				Expression.Not(
					Expression.Call(typeof (object),
									"ReferenceEquals",
									System.Type.EmptyTypes,
									ArrayIndex(Expression.Property(t, Tuple.ItemsProperty), index),
									Expression.Constant(null))),
				t);
		}

		private static Expression GetIdentifier(ISessionFactory sessionFactory, Expression expression)
		{
			var classMetadata = sessionFactory.GetClassMetadata(expression.Type);
			if (classMetadata == null)
				return Expression.Constant(null);

            var propertyName=classMetadata.IdentifierPropertyName;
            NHibernate.Type.EmbeddedComponentType componentType;
            if (propertyName == null && (componentType=classMetadata.IdentifierType as NHibernate.Type.EmbeddedComponentType)!=null)
            {
                //The identifier is an embedded composite key. We only need one property from it for a null check
                propertyName = componentType.PropertyNames.First();
            }

            return ConvertToObject(Expression.PropertyOrField(expression, propertyName));
		}

		private static LambdaExpression CreateSelector(IEnumerable<ExpressionHolder> expressions, int tuple)
		{
			var parameter = Expression.Parameter(typeof (object[]), "x");

			var initializers = expressions.Select((x, index) => new { x.Tuple, index})
				.Where(x => x.Tuple == tuple)
				.Select(x => ArrayIndex(parameter, x.index));

			var newArrayInit = Expression.NewArrayInit(typeof (object), initializers);

			return Expression.Lambda(
				Expression.New(Tuple.Constructor, newArrayInit),
				parameter);
		}

		private static Expression ArrayIndex(Expression param, int value)
		{
			return Expression.ArrayIndex(param, Expression.Constant(value));
		}

		private static Expression ConvertToObject(Expression expression)
		{
			return Expression.Convert(expression, typeof (object));
		}

		private static System.Type GetElementType(System.Type type)
		{
			var elementType = ReflectHelper.GetCollectionElementType(type);
			if (elementType == null)
				throw new NotSupportedException("Unknown collection type " + type.FullName);
			return elementType;
	}
	}
}