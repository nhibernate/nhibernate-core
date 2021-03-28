using System;
using NHibernate.Cache;
using NHibernate.Util;

namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface ICacheConfiguration
	{
		ICacheConfiguration Through<TProvider>() where TProvider : ICacheProvider;
		ICacheConfiguration PrefixingRegionsWith(string regionPrefix);
		ICacheConfiguration UsingMinimalPuts();
		IFluentSessionFactoryConfiguration WithDefaultExpiration(int seconds);
		IQueryCacheConfiguration Queries { get; }
	}

	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface ICacheConfigurationProperties
	{
		bool UseMinimalPuts { set; }
		bool UseQueryCache { set; }
		string RegionsPrefix { set; }
		int DefaultExpiration { set; }
		void Provider<TProvider>() where TProvider : ICacheProvider;
		[Obsolete("This method is invalid and should not be used. Use the QueryCacheFactory extension method instead.", true)]
		void QueryCache<TFactory>() where TFactory : IQueryCache;
	}

	// 6.0 TODO: Remove
	public static class CacheConfigurationPropertiesExtensions
	{
		//Since 5.3
		[Obsolete("Replaced by direct class usage")]
		public static void QueryCacheFactory<TFactory>(this ICacheConfigurationProperties config) where TFactory : IQueryCacheFactory
		{
			ReflectHelper
				.CastOrThrow<CacheConfigurationProperties>(config, "Setting the query cache factory with Loquacious")
				.QueryCacheFactory<TFactory>();
		}
	}
}
