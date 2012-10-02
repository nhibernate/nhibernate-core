using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class DynamicComponentMapper : IDynamicComponentMapper
	{
		private readonly HbmDynamicComponent component;
		private readonly IAccessorPropertyMapper accessorPropertyMapper;

		public DynamicComponentMapper(HbmDynamicComponent component, MemberInfo declaringTypeMember, HbmMapping mapDoc)
			: this(component, declaringTypeMember, new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x), mapDoc) {}

		private DynamicComponentMapper(HbmDynamicComponent component, MemberInfo declaringTypeMember, IAccessorPropertyMapper accessorMapper, HbmMapping mapDoc)
		{
			if (mapDoc == null)
			{
				throw new ArgumentNullException("mapDoc");
			}
			Container = declaringTypeMember.DeclaringType;
			MapDoc = mapDoc;

			this.component = component;
			accessorPropertyMapper = accessorMapper;
		}

		private System.Type Container { get; set; }
		private HbmMapping MapDoc { get; set; }

		protected void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmProperty
							  {
								name = property.Name, 
													type1 = property.GetPropertyOrFieldType().GetNhTypeName()
							  };

			mapping(new PropertyMapper(property, hbmProperty, new NoMemberPropertyMapper()));
			AddProperty(hbmProperty);
		}

		public void Component(MemberInfo property, Action<IComponentMapper> mapping)
		{
			var hbm = new HbmComponent { name = property.Name };
			mapping(new ComponentMapper(hbm, property.GetPropertyOrFieldType(), new NoMemberPropertyMapper(), MapDoc));
			AddProperty(hbm);
		}

		public void Component(MemberInfo property, Action<IDynamicComponentMapper> mapping)
		{
			var hbm = new HbmDynamicComponent { name = property.Name };
			mapping(new DynamicComponentMapper(hbm, property, new NoMemberPropertyMapper(), MapDoc));
			AddProperty(hbm);
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmManyToOne { name = property.Name };
			mapping(new ManyToOneMapper(property, new NoMemberPropertyMapper(), hbm, MapDoc));
			AddProperty(hbm);
		}

		public void Any(MemberInfo property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
		{
			var hbm = new HbmAny { name = property.Name };
			mapping(new AnyMapper(property, idTypeOfMetaType, new NoMemberPropertyMapper(), hbm, MapDoc));
			AddProperty(hbm);
		}

		public void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping)
		{
			var hbm = new HbmOneToOne { name = property.Name };

			var type = typeof (OneToOneMapper<>).MakeGenericType(property.GetPropertyOrFieldType());
			var mapper = (IOneToOneMapper) Activator.CreateInstance(type, property, new NoMemberPropertyMapper(), hbm);
			mapping(mapper);
			AddProperty(hbm);
		}

		public void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmBag { name = property.Name };
			System.Type propertyType = property.GetPropertyOrFieldType();
			System.Type collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new BagMapper(Container, collectionElementType, new NoMemberPropertyMapper(), hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmSet { name = property.Name };
			System.Type propertyType = property.GetPropertyOrFieldType();
			System.Type collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new SetMapper(Container, collectionElementType, new NoMemberPropertyMapper(), hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmList { name = property.Name };
			System.Type propertyType = property.GetPropertyOrFieldType();
			System.Type collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new ListMapper(Container, collectionElementType, new NoMemberPropertyMapper(), hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		public void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping, Action<IMapKeyRelation> keyMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmMap { name = property.Name };
			System.Type propertyType = property.GetPropertyOrFieldType();
			System.Type dictionaryKeyType = propertyType.DetermineDictionaryKeyType();
			System.Type dictionaryValueType = propertyType.DetermineDictionaryValueType();

			collectionMapping(new MapMapper(Container, dictionaryKeyType, dictionaryValueType, new NoMemberPropertyMapper(), hbm, MapDoc));
			keyMapping(new MapKeyRelation(dictionaryKeyType, hbm, MapDoc));
			mapping(new CollectionElementRelation(dictionaryValueType, MapDoc, rel => hbm.Item1 = rel));
			AddProperty(hbm);
		}

		public void IdBag(MemberInfo property, Action<IIdBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			var hbm = new HbmIdbag { name = property.Name };
			System.Type propertyType = property.GetPropertyOrFieldType();
			System.Type collectionElementType = propertyType.DetermineCollectionElementType();
			collectionMapping(new IdBagMapper(Container, collectionElementType, new NoMemberPropertyMapper(), hbm));
			mapping(new CollectionElementRelation(collectionElementType, MapDoc, rel => hbm.Item = rel));
			AddProperty(hbm);
		}

		public void Access(Accessor accessor)
		{
			accessorPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			accessorPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			component.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		public void Update(bool consideredInUpdateQuery)
		{
			component.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			component.insert = consideredInInsertQuery;
		}

		public void Unique(bool unique)
		{
			component.unique = unique;
		}
	}
}