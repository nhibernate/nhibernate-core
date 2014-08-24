using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class OneToManyMapper : IOneToManyMapper
	{
		private readonly System.Type collectionElementType;
		private readonly HbmMapping mapDoc;
		private readonly HbmOneToMany oneToManyMapping;

		public OneToManyMapper(System.Type collectionElementType, HbmOneToMany oneToManyMapping, HbmMapping mapDoc)
		{
			if (oneToManyMapping == null)
			{
				throw new ArgumentNullException("oneToManyMapping");
			}
			this.collectionElementType = collectionElementType;
			if (collectionElementType != null)
			{
				oneToManyMapping.@class = collectionElementType.GetShortClassName(mapDoc);
			}
			this.oneToManyMapping = oneToManyMapping;
			this.mapDoc = mapDoc;
		}

		#region Implementation of IOneToManyMapper

		public void Class(System.Type entityType)
		{
			if (!collectionElementType.IsAssignableFrom(entityType))
			{
				throw new ArgumentOutOfRangeException("entityType",
				                                      string.Format("The type is incompatible; expected assignable to {0}",
				                                                    collectionElementType));
			}
			oneToManyMapping.@class = entityType.GetShortClassName(mapDoc);
		}

		public void EntityName(string entityName)
		{
			oneToManyMapping.entityname = entityName;
		}

		public void NotFound(NotFoundMode mode)
		{
			if (mode == null)
			{
				return;
			}
			oneToManyMapping.notfound = mode.ToHbm();
		}

		#endregion
	}
}