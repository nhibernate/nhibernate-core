namespace NHibernate.Cfg.Loquacious
{
	public interface IQueryCacheConfiguration
	{
		// 6.0 TODO: enable constraint
		ICacheConfiguration Through<TFactory>(); // where TFactory : IQueryCacheFactory;
	}
}
