using System;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public abstract class AbstractPropertyContainerMapper : AbstractBasePropertyContainerMapper, IPropertyContainerMapper
	{
		protected AbstractPropertyContainerMapper(System.Type container, HbmMapping mapDoc) : base(container, mapDoc) {}

		#region Implementation of IPropertyContainerMapper

		public virtual void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping)
		{
			var hbm = new HbmOneToOne {name = property.Name};
			var type = typeof(OneToOneMapper<>).MakeGenericType(property.GetPropertyOrFieldType());
			var mapper = (IOneToOneMapper)Activator.CreateInstance(type, property, hbm);
			mapping(mapper);
			AddProperty(hbm);
		}

		public virtual void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmSet {name = property.Name};
			System.Type collectionElementType = property.DetermineRequiredCollectionElementType();
			collectionMapping(new SetMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public virtual void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmBag {name = property.Name};
			System.Type collectionElementType = property.DetermineRequiredCollectionElementType();
			collectionMapping(new BagMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public virtual void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmList {name = property.Name};
			System.Type collectionElementType = property.DetermineRequiredCollectionElementType();
			collectionMapping(new ListMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		public virtual void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping,
								Action<IMapKeyRelation> keyMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmMap {name = property.Name};
			System.Type propertyType = property.GetPropertyOrFieldType();
			System.Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			System.Type dictionaryValueType = propertyType.DetermineDictionaryValueType();

			collectionMapping(new MapMapper(container, dictionaryKeyType, dictionaryValueType, hbm, mapDoc));
			keyMapping(new MapKeyRelation(dictionaryKeyType, hbm, mapDoc));
			mapping(new CollectionElementRelation(dictionaryValueType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		public virtual void IdBag(MemberInfo property, Action<IIdBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmIdbag { name = property.Name };
			System.Type collectionElementType = property.DetermineRequiredCollectionElementType();
			collectionMapping(new IdBagMapper(container, collectionElementType, hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		#endregion
	}
}