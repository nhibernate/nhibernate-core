using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq
{
	/// <summary>
	/// NHibernate LINQ extension methods. They are meant to work with <see cref="NhQueryable{T}"/>. Supplied <see cref="IQueryable{T}"/> parameters
	/// should at least have an <see cref="INhQueryProvider"/> <see cref="IQueryable.Provider"/>. <see cref="LinqExtensionMethods.Query{T}(ISession)"/> and
	/// its overloads supply such queryables.
	/// </summary>
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

		private static readonly MethodInfo CacheableDefinition = ReflectHelper.GetMethodDefinition(() => Cacheable<object>(null));

		public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
		{
			var method = CacheableDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression);

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		private static readonly MethodInfo CacheModeDefinition = ReflectHelper.GetMethodDefinition(() => CacheMode<object>(null, NHibernate.CacheMode.Normal));

		public static IQueryable<T> CacheMode<T>(this IQueryable<T> query, CacheMode cacheMode)
		{
			var method = CacheModeDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(cacheMode));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		private static readonly MethodInfo CacheRegionDefinition = ReflectHelper.GetMethodDefinition(() => CacheRegion<object>(null, null));

		public static IQueryable<T> CacheRegion<T>(this IQueryable<T> query, string region)
		{
			var method = CacheRegionDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(region));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		private static readonly MethodInfo TimeoutDefinition = ReflectHelper.GetMethodDefinition(() => Timeout<object>(null, 0));

		public static IQueryable<T> Timeout<T>(this IQueryable<T> query, int timeout)
		{
			var method = TimeoutDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(timeout));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		/// <summary>
		/// Wraps the query in a deferred <see cref="IEnumerable{T}"/> which enumeration will trigger a batch of all pending future queries.
		/// </summary>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to convert to a future query.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A <see cref="IEnumerable{T}"/>.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is <see langword="null"/>.</exception>
		/// <exception cref="T:System.NotSupportedException"><paramref name="source" /> <see cref="IQueryable.Provider"/> is not a <see cref="INhQueryProvider"/>.</exception>
		public static IEnumerable<TSource> ToFuture<TSource>(this IQueryable<TSource> source)
		{
			var provider = GetNhProvider(source);
			return provider.ExecuteFuture<TSource>(source.Expression);
		}

		/// <summary>
		/// Wraps the query in a deferred <see cref="IFutureValue{T}"/> which will trigger a batch of all pending future queries
		/// when its <see cref="IFutureValue{T}.Value"/> is read.
		/// </summary>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to convert to a future query.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <returns>A <see cref="IFutureValue{T}"/>.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is <see langword="null"/>.</exception>
		/// <exception cref="T:System.NotSupportedException"><paramref name="source" /> <see cref="IQueryable.Provider"/> is not a <see cref="INhQueryProvider"/>.</exception>
		public static IFutureValue<TSource> ToFutureValue<TSource>(this IQueryable<TSource> source)
		{
			var provider = GetNhProvider(source);
			var future = provider.ExecuteFuture<TSource>(source.Expression);
			return new FutureValue<TSource>(() => future);
		}

		/// <summary>
		/// Wraps the query in a deferred <see cref="IFutureValue{T}"/> which will trigger a batch of all pending future queries
		/// when its <see cref="IFutureValue{T}.Value"/> is read.
		/// </summary>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to convert to a future query.</param>
		/// <param name="selector">An aggregation function to apply to <paramref name="source"/>.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by the function represented by <paramref name="selector"/>.</typeparam>
		/// <returns>A <see cref="IFutureValue{T}"/>.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is <see langword="null"/>.</exception>
		/// <exception cref="T:System.NotSupportedException"><paramref name="source" /> <see cref="IQueryable.Provider"/> is not a <see cref="INhQueryProvider"/>.</exception>
		public static IFutureValue<TResult> ToFutureValue<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, TResult>> selector)
		{
			var provider = GetNhProvider(source);

			var expression = ReplacingExpressionTreeVisitor
				.Replace(selector.Parameters.Single(), source.Expression, selector.Body);

			return provider.ExecuteFutureValue<TResult>(expression);
		}

		/// <summary>
		/// Deletes all entities selected by the specified query. The delete operation is performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to delete.</param>
		/// <returns>The number of deleted entities.</returns>
		public static int Delete<TSource>(this IQueryable<TSource> source)
		{
			var provider = GetNhProvider(source);
			return provider.ExecuteDelete(source.Expression);
		}

		/// <summary>
		/// Initiate an update for the entities selected by the query. The update operation will be performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <param name="expression"></param>
		/// <returns>An update builder, allowing to specify assignments to the entities properties.</returns>
		public static int Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource>> expression)
		{
			var provider = GetNhProvider(source);
			var assignments = Assignments<TSource, TSource>.FromExpression(expression);
			return provider.ExecuteUpdate(source.Expression, assignments, false);
		}

		/// <summary>
		/// Initiate an update for the entities selected by the query. The update operation will be performed in the database without reading the entities out of it.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching the entities to update.</param>
		/// <param name="expression"></param>
		/// <returns>An update builder, allowing to specify assignments to the entities properties.</returns>
		public static int UpdateVersioned<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource>> expression)
		{
			var provider = GetNhProvider(source);
			var assignments = Assignments<TSource, TSource>.FromExpression(expression);
			return provider.ExecuteUpdate(source.Expression, assignments, true);
		}

		/// <summary>
		/// Initiate an insert using selected entities as a source. Will use <c>INSERT INTO [...] SELECT FROM [...]</c> in the database.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="source">The query matching entities source of the data to insert.</param>
		/// <param name="expression"></param>
		/// <returns>An insert builder, allowing to specify target entity class and assignments to its properties.</returns>
		public static int Insert<TSource, TTarget>(
			this IQueryable<TSource> source,
			Expression<Func<TSource, TTarget>> expression)
		{
			var provider = GetNhProvider(source);
			var assignments = Assignments<TSource, TTarget>.FromExpression(expression);
			return provider.ExecuteInsert(source.Expression, assignments);
		}

		public static T MappedAs<T>(this T parameter, IType type)
		{
			throw new InvalidOperationException("The method should be used inside Linq to indicate a type of a parameter");
		}

		private static INhQueryProvider GetNhProvider<TInput>(IQueryable<TInput> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (!(source.Provider is INhQueryProvider provider))
			{
				throw new NotSupportedException($"Source {nameof(source.Provider)} must be a {nameof(INhQueryProvider)}");
			}
			return provider;
		}
	}
}
