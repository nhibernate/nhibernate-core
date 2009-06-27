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
}