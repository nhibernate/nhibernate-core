using System;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Cfg.Loquacious
{
	internal class EntityCacheConfigurationProperties<TEntity> : IEntityCacheConfigurationProperties<TEntity>
		where TEntity : class
	{
		private readonly Dictionary<string, IEntityCollectionCacheConfigurationProperties> collections;

		public EntityCacheConfigurationProperties()
		{
			collections = new Dictionary<string, IEntityCollectionCacheConfigurationProperties>(10);
			Strategy = null;
		}

		#region Implementation of IEntityCacheConfigurationProperties

		public EntityCacheUsage? Strategy { set; get; }
		public string RegionName { set; get; }

		public void Collection<TCollection>(Expression<Func<TEntity, TCollection>> collectionProperty,
																				Action<IEntityCollectionCacheConfigurationProperties> collectionCacheConfiguration)
			where TCollection : IEnumerable
		{
			if (collectionProperty == null)
			{
				throw new ArgumentNullException("collectionProperty");
			}
			var mi = ExpressionsHelper.DecodeMemberAccessExpression(collectionProperty);
			if(mi.DeclaringType != typeof(TEntity))
			{
				throw new ArgumentOutOfRangeException("collectionProperty", "Collection not owned by " + typeof (TEntity).FullName);
			}
			var ecc = new EntityCollectionCacheConfigurationProperties();
			collectionCacheConfiguration(ecc);
			collections.Add(typeof (TEntity).FullName + "." + mi.Name, ecc);
		}

		#endregion

		public IDictionary<string, IEntityCollectionCacheConfigurationProperties> Collections
		{
			get { return collections; }
		}
	}

	internal class EntityCollectionCacheConfigurationProperties : IEntityCollectionCacheConfigurationProperties
	{
		public EntityCollectionCacheConfigurationProperties()
		{
			Strategy = EntityCacheUsage.ReadWrite;
		}

		#region Implementation of IEntityCollectionCacheConfigurationProperties

		public EntityCacheUsage Strategy { get; set; }
		public string RegionName { get; set; }

		#endregion
	}
}