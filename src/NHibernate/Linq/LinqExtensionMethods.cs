using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionVisitors;

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

			var expression = ReplacingExpressionVisitor
				.Replace(selector.Parameters.Single(), source.Expression, selector.Body);

			return provider.ExecuteFutureValue<TResult>(expression);
		}


		internal static readonly MethodInfo SetOptionsDefinition =
			ReflectHelper.GetMethodDefinition(() => SetOptions<object>(null, null));

		/// <summary>
		/// Allow to set NHibernate query options.
		/// </summary>
		/// <typeparam name="T">The type of the queried elements.</typeparam>
		/// <param name="query">The query on which to set options.</param>
		/// <param name="setOptions">The options setter.</param>
		/// <returns>The query altered with the options.</returns>
		public static IQueryable<T> SetOptions<T>(this IQueryable<T> query, Action<IQueryableOptions> setOptions)
		{
			var method = SetOptionsDefinition.MakeGenericMethod(typeof(T));
			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(setOptions));
			return new NhQueryable<T>(query.Provider, callExpression);
		}

		[Obsolete("Please use SetOptions instead.")]
		public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
			=> query.SetOptions(o => o.SetCacheable(true));

		[Obsolete("Please use SetOptions instead.")]
		public static IQueryable<T> CacheMode<T>(this IQueryable<T> query, CacheMode cacheMode)
			=> query.SetOptions(o => o.SetCacheMode(cacheMode));

		[Obsolete("Please use SetOptions instead.")]
		public static IQueryable<T> CacheRegion<T>(this IQueryable<T> query, string region)
			=> query.SetOptions(o => o.SetCacheRegion(region));

		[Obsolete("Please use SetOptions instead.")]
		public static IQueryable<T> Timeout<T>(this IQueryable<T> query, int timeout)
			=> query.SetOptions(o => o.SetTimeout(timeout));

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
		/// <returns>An update builder, allowing to specify assignments to the entities properties.</returns>
		public static UpdateSyntax<TSource> Update<TSource>(this IQueryable<TSource> source)
		{
			var provider = GetNhProvider(source);
			return new UpdateSyntax<TSource>(source.Expression, provider);
		}

		/// <summary>
		/// Initiate an insert using selected entities as a source. Will use <c>INSERT INTO [...] SELECT FROM [...]</c> in the database.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <param name="source">The query matching entities source of the data to insert.</param>
		/// <returns>An insert builder, allowing to specify target entity class and assignments to its properties.</returns>
		public static InsertSyntax<TSource> Insert<TSource>(this IQueryable<TSource> source)
		{
			var provider = GetNhProvider(source);
			return new InsertSyntax<TSource>(source.Expression, provider);
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
