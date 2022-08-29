using System;
using System.Collections;
using System.Linq.Expressions;

namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IEntityCollectionCacheConfigurationProperties
	{
		EntityCacheUsage Strategy { get; set; }
		string RegionName { get; set; }
	}

	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IEntityCacheConfigurationProperties<TEntity> where TEntity : class
	{
		EntityCacheUsage? Strategy { get; set; }
		string RegionName { get; set; }

		void Collection<TCollection>(Expression<Func<TEntity, TCollection>> collectionProperty, Action<IEntityCollectionCacheConfigurationProperties> collectionCacheConfiguration)
			where TCollection : IEnumerable;
	}
}
