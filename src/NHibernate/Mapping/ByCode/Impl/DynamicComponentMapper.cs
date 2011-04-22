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

		public DynamicComponentMapper(HbmDynamicComponent component, MemberInfo declaringTypeMember, HbmMapping mapDoc) : base(declaringTypeMember.DeclaringType, mapDoc)
		{
			this.component = component;
			accessorPropertyMapper = new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x);
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