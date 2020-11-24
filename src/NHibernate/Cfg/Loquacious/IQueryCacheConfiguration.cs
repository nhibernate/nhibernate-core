using System;

namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IQueryCacheConfiguration
	{
		// 6.0 TODO: enable constraint
		ICacheConfiguration Through<TFactory>(); // where TFactory : IQueryCacheFactory;
	}
}
