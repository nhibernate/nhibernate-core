using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class DynamicComponentMapper : AbstractPropertyContainerMapper, IDynamicComponentMapper
	{
		private readonly HbmDynamicComponent component;
		private readonly IAccessorPropertyMapper accessorPropertyMapper;

		public DynamicComponentMapper(HbmDynamicComponent component, MemberInfo declaringTypeMember, HbmMapping mapDoc)
			: this(component, declaringTypeMember, new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x), mapDoc) {}

		private DynamicComponentMapper(HbmDynamicComponent component, MemberInfo declaringTypeMember, IAccessorPropertyMapper accessorMapper, HbmMapping mapDoc)
			: base(declaringTypeMember.DeclaringType, mapDoc)
		{
			this.component = component;
			accessorPropertyMapper = accessorMapper;
		}

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}

		public override void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmProperty
			                  {
			                  	name = property.Name, 
													type1 = property.GetPropertyOrFieldType().GetNhTypeName()
			                  };

			mapping(new PropertyMapper(property, hbmProperty, new NoMemberPropertyMapper()));
			AddProperty(hbmProperty);
		}

		public override void Component(MemberInfo property, Action<IComponentMapper> mapping)
		{
			var hbm = new HbmComponent { name = property.Name };
			mapping(new ComponentMapper(hbm, property.GetPropertyOrFieldType(), new NoMemberPropertyMapper(), MapDoc));
			AddProperty(hbm);
		}

		public override void Component(MemberInfo property, Action<IDynamicComponentMapper> mapping)
		{
			var hbm = new HbmDynamicComponent { name = property.Name };
			mapping(new DynamicComponentMapper(hbm, property, new NoMemberPropertyMapper(), MapDoc));
			AddProperty(hbm);
		}

		public override void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmManyToOne { name = property.Name };
			mapping(new ManyToOneMapper(property, new NoMemberPropertyMapper(), hbm, MapDoc));
			AddProperty(hbm);
		}

		public override void Any(MemberInfo property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
		{
			var hbm = new HbmAny { name = property.Name };
			mapping(new AnyMapper(property, idTypeOfMetaType, new NoMemberPropertyMapper(), hbm, MapDoc));
			AddProperty(hbm);
		}

		protected override bool IsMemberSupportedByMappedContainer(MemberInfo property)
		{
			return true;
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
	}
}