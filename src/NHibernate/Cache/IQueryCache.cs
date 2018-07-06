using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines the contract for caches capable of storing query results.  These
	/// caches should only concern themselves with storing the matching result ids
	/// of entities.
	/// The transactional semantics are necessarily less strict than the semantics
	/// of an item cache.
	/// <see cref="IBatchableQueryCache" /> should also be implemented for
	/// compatibility with future versions.
	/// </summary>
	public partial interface IQueryCache
	{
		/// <summary>
		/// The underlying <see cref="ICache"/>.
		/// </summary>
		ICache Cache { get; }
		/// <summary>
		/// The cache region.
		/// </summary>
		string RegionName { get; }

		/// <summary>
		/// Clear the cache.
		/// </summary>
		void Clear();
		// Since 5.2
		[Obsolete("Have the query cache implement IBatchableQueryCache, and use IBatchableQueryCache.Put")]
		bool Put(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session);
		// Since 5.2
		[Obsolete("Have the query cache implement IBatchableQueryCache, and use IBatchableQueryCache.Get")]
		IList Get(QueryKey key, ICacheAssembler[] returnTypes, bool isNaturalKeyLookup, ISet<string> spaces, ISessionImplementor session);
		/// <summary>
		/// Clean up all resources.
		/// </summary>
		void Destroy();
	}

	// 6.0 TODO: merge into IQueryCache
	/// <summary>
	/// Transitional interface for <see cref="IQueryCache" />.
	/// </summary>
	public partial interface IBatchableQueryCache : IQueryCache
	{
		/// <summary>
		/// Get query results from the cache.
		/// </summary>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="spaces">The query spaces.</param>
		/// <param name="session">The session for which the query is executed.</param>
		/// <returns>The query results, if cached.</returns>
		IList Get(
			QueryKey key, QueryParameters queryParameters, ICacheAssembler[] returnTypes, ISet<string> spaces,
			ISessionImplementor session);

		/// <summary>
		/// Put query results in the cache.
		/// </summary>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="result">The query result.</param>
		/// <param name="session">The session for which the query was executed.</param>
		/// <returns><see langword="true" /> if the result has been cached, <see langword="false" />
		/// otherwise.</returns>
		bool Put(
			QueryKey key, QueryParameters queryParameters, ICacheAssembler[] returnTypes, IList result,
			ISessionImplementor session);

		/// <summary>
		/// Retrieve multiple query results from the cache.
		/// </summary>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="spaces">The array of query spaces matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries are executed.</param>
		/// <returns>The cached query results, matching each key of <paramref name="keys"/> respectively. For each
		/// missed key, it will contain a <see langword="null" />.</returns>
		IList[] GetMany(
			QueryKey[] keys, QueryParameters[] queryParameters, ICacheAssembler[][] returnTypes,
			ISet<string>[] spaces, ISessionImplementor session);

		/// <summary>
		/// Attempt to cache objects, after loading them from the database.
		/// </summary>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="results">The array of query results matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries were executed.</param>
		/// <returns>An array of boolean indicating if each query was successfully cached.</returns>
		/// <exception cref="CacheException"></exception>
		bool[] PutMany(
			QueryKey[] keys, QueryParameters[] queryParameters, ICacheAssembler[][] returnTypes, IList[] results,
			ISessionImplementor session);
	}

	// 6.0 TODO: drop
	internal static partial class QueryCacheExtensions
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(QueryCacheExtensions));

		// Non thread safe: not an issue, at worst it will cause a few more logs than one.
		// Does not handle the possibility of using multiple diffreent obsoleted query cache implementation:
		// only the first encountered will be logged.
		private static bool _hasWarnForObsoleteQueryCache;

		/// <summary>
		/// Get query results from the cache.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="spaces">The query spaces.</param>
		/// <param name="session">The session for which the query is executed.</param>
		/// <returns>The query results, if cached.</returns>
		public static IList Get(
			this IQueryCache queryCache,
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			ISet<string> spaces,
			ISessionImplementor session)
		{
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return batchableQueryCache.Get(
					key,
					queryParameters,
					returnTypes,
					spaces,
					session);
			}

			if (!_hasWarnForObsoleteQueryCache)
			{
				_hasWarnForObsoleteQueryCache = true;
				Log.Warn("{0} is obsolete, it should implement {1}", queryCache, nameof(IBatchableQueryCache));
			}

			var persistenceContext = session.PersistenceContext;

			var defaultReadOnlyOrig = persistenceContext.DefaultReadOnly;

			if (queryParameters.IsReadOnlyInitialized)
				persistenceContext.DefaultReadOnly = queryParameters.ReadOnly;
			else
				queryParameters.ReadOnly = persistenceContext.DefaultReadOnly;

			try
			{
#pragma warning disable 618
				return queryCache.Get(
#pragma warning restore 618
					key,
					returnTypes,
					queryParameters.NaturalKeyLookup,
					spaces,
					session);
			}
			finally
			{
				persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
			}
		}

		/// <summary>
		/// Put query results in the cache.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="key">The query key.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="returnTypes">The query result row types.</param>
		/// <param name="result">The query result.</param>
		/// <param name="session">The session for which the query was executed.</param>
		/// <returns><see langword="true" /> if the result has been cached, <see langword="false" />
		/// otherwise.</returns>
		public static bool Put(
			this IQueryCache queryCache,
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			IList result,
			ISessionImplementor session)
		{
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return batchableQueryCache.Put(
					key, queryParameters,
					returnTypes,
					result, session);
			}

#pragma warning disable 618
			return queryCache.Put(
#pragma warning restore 618
				key,
				returnTypes,
				result,
				queryParameters.NaturalKeyLookup,
				session);
		}

		/// <summary>
		/// Retrieve multiple query results from the cache.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="spaces">The array of query spaces matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries are executed.</param>
		/// <returns>The cached query results, matching each key of <paramref name="keys"/> respectively. For each
		/// missed key, it will contain a <see langword="null" />.</returns>
		public static IList[] GetMany(
			this IQueryCache queryCache,
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			ISet<string>[] spaces,
			ISessionImplementor session)
		{
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return batchableQueryCache.GetMany(
					keys,
					queryParameters,
					returnTypes,
					spaces,
					session);
			}

			var results = new IList[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				results[i] = queryCache.Get(keys[i], queryParameters[i], returnTypes[i], spaces[i], session);
			}

			return results;
		}

		/// <summary>
		/// Attempt to cache objects, after loading them from the database.
		/// </summary>
		/// <param name="queryCache">The cache.</param>
		/// <param name="keys">The query keys.</param>
		/// <param name="queryParameters">The array of query parameters matching <paramref name="keys"/>.</param>
		/// <param name="returnTypes">The array of query result row types matching <paramref name="keys"/>.</param>
		/// <param name="results">The array of query results matching <paramref name="keys"/>.</param>
		/// <param name="session">The session for which the queries were executed.</param>
		/// <returns>An array of boolean indicating if each query was successfully cached.</returns>
		/// <exception cref="CacheException"></exception>
		public static bool[] PutMany(
			this IQueryCache queryCache,
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			IList[] results,
			ISessionImplementor session)
		{
			if (queryCache is IBatchableQueryCache batchableQueryCache)
			{
				return batchableQueryCache.PutMany(
					keys,
					queryParameters,
					returnTypes,
					results,
					session);
			}

			var puts = new bool[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				puts[i] = queryCache.Put(keys[i], queryParameters[i], returnTypes[i], results[i], session);
			}

			return puts;
		}
	}
}
