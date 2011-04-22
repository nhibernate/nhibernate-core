using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentMapper : AbstractPropertyContainerMapper, IComponentMapper
	{
		private readonly IAccessorPropertyMapper accessorPropertyMapper;
		private readonly HbmComponent component;
		private ComponentParentMapper parentMapper;

		public ComponentMapper(HbmComponent component, System.Type componentType, MemberInfo declaringTypeMember, HbmMapping mapDoc)
			: this(component,componentType, new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x), mapDoc)
		{
		}

		public ComponentMapper(HbmComponent component, System.Type componentType, IAccessorPropertyMapper accessorMapper, HbmMapping mapDoc)
			: base(componentType, mapDoc)
		{
			this.component = component;
			component.@class = componentType.GetShortClassName(mapDoc);
			accessorPropertyMapper = accessorMapper;
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] {property};
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}

		#endregion

		#region Implementation of IComponentMapper

		public void Parent(MemberInfo parent)
		{
			Parent(parent, x => { });
		}

		public void Parent(MemberInfo parent, Action<IComponentParentMapper> parentMapping)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			IComponentParentMapper mapper = GetParentMapper(parent);
			parentMapping(mapper);
		}

		public void Update(bool consideredInUpdateQuery)
		{
			component.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			component.insert = consideredInInsertQuery;
		}

		public void Lazy(bool isLazy)
		{
			component.lazy = isLazy;
		}

		public void Class(System.Type componentType)
		{
			component.@class = componentType.GetShortClassName(mapDoc);
		}

		#endregion

		#region IComponentMapper Members

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

		#endregion

		private IComponentParentMapper GetParentMapper(MemberInfo parent)
		{
			if (parentMapper != null)
			{
				return parentMapper;
			}
			component.parent = new HbmParent();
			return parentMapper = new ComponentParentMapper(component.parent, parent);
		}
	}
}