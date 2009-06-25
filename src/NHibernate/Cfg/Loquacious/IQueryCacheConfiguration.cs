using NHibernate.Cache;

namespace NHibernate.Cfg.Loquacious
{
	public interface IQueryCacheConfiguration
	{
		ICacheConfiguration Trough<TFactory>() where TFactory : IQueryCache;
	}
}