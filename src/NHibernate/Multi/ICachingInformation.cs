using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Multi
{
	/// <summary>
	/// Querying information.
	/// </summary>
	public interface ICachingInformation
	{
		/// <summary>
		/// Is the query cacheable?
		/// </summary>
		bool IsCacheable { get; }

		/// <summary>
		/// The query cache key.
		/// </summary>
		QueryKey CacheKey { get; }

		/// <summary>
		/// The query parameters.
		/// </summary>
		QueryParameters Parameters { get; }

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
		/// Can the query be obtained from cache?
		/// </summary>
		bool CanGetFromCache { get; }

		/// <summary>
		/// The query result types.
		/// </summary>
		// Since 5.3
		[Obsolete("This property is not used and will be removed in a future version.")]
		IType[] ResultTypes { get; }

		/// <summary>
		/// The query result to put in the cache. <see langword="null" /> if no put should be done.
		/// </summary>
		IList ResultToCache { get; }

		/// <summary>
		/// The query identifier, for statistics purpose.
		/// </summary>
		string QueryIdentifier { get; }

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

	internal static class CachingInformationExtensions
	{
		// 6.0 TODO: Remove and use CacheTypes instead.
		public static IType[] GetCacheTypes(this ICachingInformation cachingInformation)
		{
			if (cachingInformation is ICachingInformationWithFetches cachingInformationWithFetches)
			{
				return cachingInformationWithFetches.CacheTypes;
			}

#pragma warning disable 618
			return cachingInformation.ResultTypes;
#pragma warning restore 618
		}
	}
}
