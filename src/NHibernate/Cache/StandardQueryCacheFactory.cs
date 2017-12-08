using System;
using System.Collections.Generic;
using NHibernate.Cfg;

namespace NHibernate.Cache
{
	/// <summary>
	/// Standard Hibernate implementation of the IQueryCacheFactory interface.  Returns
	/// instances of <see cref="StandardQueryCache" />.
	/// </summary>
	public class StandardQueryCacheFactory : IQueryCacheFactory
	{
		// Since v5.2
		[Obsolete("Please use overload with an CacheBase parameter.")]
		public IQueryCache GetQueryCache(string regionName,
																		 UpdateTimestampsCache updateTimestampsCache,
																		 Settings settings,
																		 IDictionary<string, string> props)
		{
			return new StandardQueryCache(settings, props, updateTimestampsCache, regionName);
		}

		/// <summary>
		/// Build a query cache.
		/// </summary>
		/// <param name="updateTimestampsCache">The cache of updates timestamps.</param>
		/// <param name="props">The NHibernate settings properties.</param>
		/// <param name="regionCache">The <see cref="CacheBase" /> to use for the region.</param>
		/// <returns>A query cache.</returns>
		public virtual IQueryCache GetQueryCache(
			UpdateTimestampsCache updateTimestampsCache,
			IDictionary<string, string> props,
			CacheBase regionCache)
		{
			return new StandardQueryCache(updateTimestampsCache, regionCache);
		}
	}
}
