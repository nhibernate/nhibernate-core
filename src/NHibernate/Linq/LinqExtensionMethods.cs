using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Impl;
using NHibernate.Type;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq
{
	public static class LinqExtensionMethods
	{
		public static IQueryable<T> Query<T>(this ISession session)
		{
			return new NhQueryable<T>(session.GetSessionImplementation());
		}

		public static IQueryable<T> Query<T>(this ISession session, string entityName)
		{
			return new NhQueryable<T>(session.GetSessionImplementation(), entityName);
		}

		public static IQueryable<T> Query<T>(this IStatelessSession session)
		{
			return new NhQueryable<T>(session.GetSessionImplementation());
		}

		public static IQueryable<T> Query<T>(this IStatelessSession session, string entityName)
		{
			return new NhQueryable<T>(session.GetSessionImplementation(), entityName);
		}

		public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => Cacheable<object>(null)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression);

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IQueryable<T> CacheMode<T>(this IQueryable<T> query, CacheMode cacheMode)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => CacheMode<object>(null, NHibernate.CacheMode.Normal)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(cacheMode));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IQueryable<T> CacheRegion<T>(this IQueryable<T> query, string region)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => CacheRegion<object>(null, null)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(region));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IQueryable<T> Timeout<T>(this IQueryable<T> query, int timeout)
		{
			var method = ReflectionHelper.GetMethodDefinition(() => Timeout<object>(null, 0)).MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(timeout));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IEnumerable<T> ToFuture<T>(this IQueryable<T> query)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) nhQueryable.Provider;
			var future = provider.ExecuteFuture(nhQueryable.Expression);
			return (IEnumerable<T>) future;
		}

		public static IFutureValue<T> ToFutureValue<T>(this IQueryable<T> query)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) nhQueryable.Provider;
			var future = provider.ExecuteFuture(nhQueryable.Expression);
			if (future is IEnumerable<T>)
			{
				return new FutureValue<T>(() => ((IEnumerable<T>) future));
			}

			return (IFutureValue<T>) future;
		}

		public static T MappedAs<T>(this T parameter, IType type)
		{
			throw new InvalidOperationException("The method should be used inside Linq to indicate a type of a parameter");
		}

		public static IFutureValue<TResult> ToFutureValue<T, TResult>(this IQueryable<T> query, Expression<Func<IQueryable<T>, TResult>> selector)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) query.Provider;

			var expression = ReplacingExpressionTreeVisitor.Replace(selector.Parameters.Single(),
																	query.Expression,
																	selector.Body);

			return (IFutureValue<TResult>) provider.ExecuteFuture(expression);
		}
	}
}
