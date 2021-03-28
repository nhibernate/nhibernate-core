using System;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Cfg.Loquacious
{
	public class EntityCacheConfigurationProperties<TEntity>
#pragma warning disable 618
		: IEntityCacheConfigurationProperties<TEntity>
#pragma warning restore 618
		where TEntity : class
	{
		private readonly Dictionary<string, EntityCollectionCacheConfigurationProperties> collections;

		public EntityCacheConfigurationProperties()
		{
			collections = new Dictionary<string, EntityCollectionCacheConfigurationProperties>(10);
			Strategy = null;
		}

		public EntityCacheUsage? Strategy { set; get; }
		public string RegionName { set; get; }

		public void Collection<TCollection>(Expression<Func<TEntity, TCollection>> collectionProperty,
											Action<EntityCollectionCacheConfigurationProperties> collectionCacheConfiguration)
			where TCollection : IEnumerable
		{
			if (collectionProperty == null)
			{
				throw new ArgumentNullException(nameof(collectionProperty));
			}
			var mi = ExpressionsHelper.DecodeMemberAccessExpression(collectionProperty);
			if(mi.DeclaringType != typeof(TEntity))
			{
				throw new ArgumentOutOfRangeException(nameof(collectionProperty), "Collection not owned by " + typeof (TEntity).FullName);
			}
			var ecc = new EntityCollectionCacheConfigurationProperties();
			collectionCacheConfiguration(ecc);
			collections.Add(typeof (TEntity).FullName + "." + mi.Name, ecc);
		}

#pragma warning disable 618
		#region Implementation of IEntityCacheConfigurationProperties

		void IEntityCacheConfigurationProperties<TEntity>.Collection<TCollection>(Expression<Func<TEntity, TCollection>> collectionProperty,
																				Action<IEntityCollectionCacheConfigurationProperties> collectionCacheConfiguration)
		{
			Collection(collectionProperty, collectionCacheConfiguration);
		}

		#endregion
#pragma warning restore 618

		internal IDictionary<string, EntityCollectionCacheConfigurationProperties> Collections
		{
			get { return collections; }
		}
	}

	public class EntityCollectionCacheConfigurationProperties 
#pragma warning disable 618
		: IEntityCollectionCacheConfigurationProperties
#pragma warning restore 618
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
