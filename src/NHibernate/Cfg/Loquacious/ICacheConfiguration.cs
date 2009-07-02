using NHibernate.Cache;
namespace NHibernate.Cfg.Loquacious
{
	public interface ICacheConfiguration
	{
		ICacheConfiguration Through<TProvider>() where TProvider : ICacheProvider;
		ICacheConfiguration PrefixingRegionsWith(string regionPrefix);
		ICacheConfiguration UsingMinimalPuts();
		IFluentSessionFactoryConfiguration WithDefaultExpiration(byte seconds);
		IQueryCacheConfiguration Queries { get; }
	}

	public interface ICacheConfigurationProperties
	{
		bool UseMinimalPuts { set; }
		string RegionsPrefix { set; }
		byte DefaultExpiration { set; }
		void Provider<TProvider>() where TProvider : ICacheProvider;
		void QueryCache<TFactory>() where TFactory : IQueryCache;
	}
}