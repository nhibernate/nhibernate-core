using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentMapper : AbstractPropertyContainerMapper, IComponentMapper
	{
		private readonly IAccessorPropertyMapper _accessorPropertyMapper;
		private readonly HbmComponent _component;
		private ComponentParentMapper _parentMapper;

		public ComponentMapper(HbmComponent component, System.Type componentType, MemberInfo declaringTypeMember, HbmMapping mapDoc)
			: this(component,componentType, new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => component.access = x), mapDoc)
		{
		}

		public ComponentMapper(HbmComponent component, System.Type componentType, IAccessorPropertyMapper accessorMapper, HbmMapping mapDoc)
			: base(componentType, mapDoc)
		{
			_component = component;
			_component.@class = componentType.GetShortClassName(mapDoc);
			_accessorPropertyMapper = accessorMapper;
		}

		#region Overrides of AbstractPropertyContainerMapper

		protected override void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] {property};
			_component.Items = _component.Items == null ? toAdd : _component.Items.Concat(toAdd).ToArray();
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
			_component.update = consideredInUpdateQuery;
		}

		public void Insert(bool consideredInInsertQuery)
		{
			_component.insert = consideredInInsertQuery;
		}

		public void Lazy(bool isLazy)
		{
			_component.lazy = isLazy;
		}

		public void Unique(bool unique)
		{
			_component.unique = unique;
		}

		public void Class(System.Type componentType)
		{
			_component.@class = componentType.GetShortClassName(mapDoc);
		}

		#endregion

		#region IComponentMapper Members

		public void Access(Accessor accessor)
		{
			_accessorPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			_accessorPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			_component.optimisticlock = takeInConsiderationForOptimisticLock;
		}

		#endregion

		private IComponentParentMapper GetParentMapper(MemberInfo parent)
		{
			if (_parentMapper != null)
			{
				return _parentMapper;
			}
			_component.parent = new HbmParent();
			return _parentMapper = new ComponentParentMapper(_component.parent, parent);
		}
	}
}