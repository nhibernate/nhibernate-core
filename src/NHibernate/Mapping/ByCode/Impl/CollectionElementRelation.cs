using System;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class CollectionElementRelation : ICollectionElementRelation
	{
		private readonly System.Type collectionElementType;
		private readonly Action<object> elementRelationshipAssing;
		private readonly HbmMapping mapDoc;

		public CollectionElementRelation(System.Type collectionElementType, HbmMapping mapDoc, Action<object> elementRelationshipAssing)
		{
			this.collectionElementType = collectionElementType;
			this.mapDoc = mapDoc;
			this.elementRelationshipAssing = elementRelationshipAssing;
		}

		#region Implementation of ICollectionElementRelation

		public void Element(Action<IElementMapper> mapping)
		{
			var hbm = new HbmElement();
			mapping(new ElementMapper(collectionElementType, hbm));
			elementRelationshipAssing(hbm);
		}

		public void OneToMany(Action<IOneToManyMapper> mapping)
		{
			var hbm = new HbmOneToMany {@class = collectionElementType.GetShortClassName(mapDoc)};
			mapping(new OneToManyMapper(collectionElementType, hbm, mapDoc));
			elementRelationshipAssing(hbm);
		}

		public void ManyToMany(Action<IManyToManyMapper> mapping)
		{
			var hbm = new HbmManyToMany {@class = collectionElementType.GetShortClassName(mapDoc)};
			mapping(new ManyToManyMapper(collectionElementType, hbm, mapDoc));
			elementRelationshipAssing(hbm);
		}

		public void Component(Action<IComponentElementMapper> mapping)
		{
			var hbm = new HbmCompositeElement {@class = collectionElementType.GetShortClassName(mapDoc)};
			mapping(new ComponentElementMapper(collectionElementType, mapDoc, hbm));
			elementRelationshipAssing(hbm);
		}

		public void ManyToAny(System.Type idTypeOfMetaType, Action<IManyToAnyMapper> mapping)
		{
			var hbm = new HbmManyToAny();
			mapping(new ManyToAnyMapper(collectionElementType, idTypeOfMetaType, hbm, mapDoc));
			elementRelationshipAssing(hbm);
		}

		#endregion
	}
}