using NHibernate.Cache;

namespace NHibernate.Cfg.Loquacious
{
	public interface IQueryCacheConfiguration
	{
		ICacheConfiguration Through<TFactory>() where TFactory : IQueryCache;
	}
}