using NHibernate.Cache;
namespace NHibernate.Cfg.Loquacious
{
	public interface ICacheConfiguration
	{
		ICacheConfiguration Through<TProvider>() where TProvider : ICacheProvider;
		ICacheConfiguration PrefixingRegionsWith(string regionPrefix);
		ICacheConfiguration UsingMinimalPuts();
		IFluentSessionFactoryConfiguration WithDefaultExpiration(int seconds);
		IQueryCacheConfiguration Queries { get; }
	}

	public interface ICacheConfigurationProperties
	{
		bool UseMinimalPuts { set; }
		bool UseQueryCache { set; }
		string RegionsPrefix { set; }
		int DefaultExpiration { set; }
		void Provider<TProvider>() where TProvider : ICacheProvider;
		void QueryCache<TFactory>() where TFactory : IQueryCache;
	}
}