using System.Collections;
using System.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Multi
{
	/// <summary>
	/// Querying information.
	/// </summary>
	public interface ICachingInformation
	{
		/// <summary>
		/// The query parameters.
		/// </summary>
		QueryParameters Parameters { get; }

		/// <summary>
		/// The query result.
		/// </summary>
		IList Result { get; }

		/// <summary>
		/// The query spaces.
		/// </summary>
		/// <remarks>
		/// Query spaces indicates which entity classes are used by the query and need to be flushed
		/// when auto-flush is enabled. It also indicates which cache update timestamps needs to be
		/// checked for up-to-date-ness.
		/// </remarks>
		ISet<string> QuerySpaces { get; }

		/// <summary>
		/// Is the query cacheable?
		/// </summary>
		bool IsCacheable { get; }

		/// <summary>
		/// Can the query be obtained from cache?
		/// </summary>
		bool CanGetFromCache { get; }

		/// <summary>
		/// Indicates if <see cref="Result"/> was obtained from the cache.
		/// </summary>
		bool IsResultFromCache { get; }

		/// <summary>
		/// The query result types.
		/// </summary>
		IType[] ResultTypes { get; }

		/// <summary>
		/// The query identifier, for statistics purpose.
		/// </summary>
		string QueryIdentifier { get; }

		/// <summary>
		/// The query cache key.
		/// </summary>
		QueryKey CacheKey { get; }

		/// <summary>
		/// Set the result retrieved from the cache.
		/// </summary>
		/// <param name="result">The results. Can be <see langword="null" /> in case of cache miss.</param>
		void SetCachedResult(IList result);

		/// <summary>
		/// Set the <see cref="CacheBatcher" /> to use for batching entities and collections cache puts.
		/// </summary>
		/// <param name="cacheBatcher">A cache batcher.</param>
		void SetCacheBatcher(CacheBatcher cacheBatcher);
	}
}
