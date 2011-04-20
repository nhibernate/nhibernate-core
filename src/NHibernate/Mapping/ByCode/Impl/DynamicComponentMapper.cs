using System;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class DynamicComponentMapper : IDynamicComponentMapper
	{
		private readonly HbmDynamicComponent component;
		private readonly IAccessorPropertyMapper accessorPropertyMapper;

		public DynamicComponentMapper(HbmDynamicComponent component, MemberInfo declaringTypeMember, HbmMapping mapDoc)
		{
			this.component = component;
			accessorPropertyMapper = new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x);
		}

		protected void AddProperty(object property)
		{
			throw new NotImplementedException();
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

		public void Set(MemberInfo property, Action<ISetPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void Bag(MemberInfo property, Action<IBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void List(MemberInfo property, Action<IListPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void Map(MemberInfo property, Action<IMapPropertiesMapper> collectionMapping, Action<IMapKeyRelation> keyMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void IdBag(MemberInfo property, Action<IIdBagPropertiesMapper> collectionMapping, Action<ICollectionElementRelation> mapping)
		{
			throw new NotImplementedException();
		}

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void Component(MemberInfo property, Action<IComponentMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void Component(MemberInfo property, Action<IDynamicComponentMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void Any(MemberInfo property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
		{
			throw new NotImplementedException();
		}

		public void OneToOne(MemberInfo property, Action<IOneToOneMapper> mapping)
		{
			throw new NotImplementedException();
		}
	}
}