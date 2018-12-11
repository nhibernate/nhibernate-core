using System;
using System.Collections.Generic;
using NHibernate.Cfg;

namespace NHibernate.Cache
{
	/// <summary>
	/// Defines a factory for query cache instances.  These factories are responsible for
	/// creating individual QueryCache instances.
	/// </summary>
	public interface IQueryCacheFactory
	{
		// Since v5.3
		[Obsolete("Please use extension overload with a CacheBase parameter.")]
		IQueryCache GetQueryCache(
			string regionName,
			UpdateTimestampsCache updateTimestampsCache,
			Settings settings,
			IDictionary<string, string> props);
	}

	// 6.0 TODO: move to interface.
	public static class QueryCacheFactoryExtension
	{
		private static readonly INHibernateLogger Logger = NHibernateLogger.For(typeof(QueryCacheFactoryExtension));

		/// <summary>
		/// Build a query cache.
		/// </summary>
		/// <param name="factory">The query cache factory.</param>
		/// <param name="updateTimestampsCache">The cache of updates timestamps.</param>
		/// <param name="props">The NHibernate settings properties.</param>
		/// <param name="regionCache">The <see cref="ICache" /> to use for the region.</param>
		/// <returns>A query cache. <c>null</c> if <paramref name="factory"/> does not implement a
		/// <c>public IQueryCache GetQueryCache(UpdateTimestampsCache, IDictionary&lt;string, string&gt; props, CacheBase)</c>
		/// method.</returns>
		public static IQueryCache GetQueryCache(
			this IQueryCacheFactory factory,
			UpdateTimestampsCache updateTimestampsCache,
			IDictionary<string, string> props,
			CacheBase regionCache)
		{
			if (factory is StandardQueryCacheFactory standardFactory)
			{
				return standardFactory.GetQueryCache(updateTimestampsCache, props, regionCache);
			}

			// Use reflection for supporting custom factories.
			var factoryType = factory.GetType();
			var getQueryCacheMethod = factoryType.GetMethod(
				nameof(StandardQueryCacheFactory.GetQueryCache),
				new[] { typeof(UpdateTimestampsCache), typeof(IDictionary<string, string>), typeof(CacheBase) });
			if (getQueryCacheMethod != null)
			{
				return (IQueryCache) getQueryCacheMethod.Invoke(
					factory,
					new object[] { updateTimestampsCache, props, regionCache });
			}

			// Caller has to call the obsolete method.
			Logger.Warn(
				"{0} does not implement 'IQueryCache GetQueryCache(UpdateTimestampsCache, IDictionary&lt;string, " +
				"string&gt; props, CacheBase)', its obsolete overload will be used.",
				factoryType);
			return null;
		}
	}
}
