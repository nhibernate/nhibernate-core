using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentNestedElementMapper : IComponentElementMapper
	{
		private readonly IAccessorPropertyMapper _accessorPropertyMapper;
		private readonly HbmNestedCompositeElement _component;
		private readonly System.Type _componentType;
		private readonly HbmMapping _mapDoc;
		private IComponentParentMapper _parentMapper;

		public ComponentNestedElementMapper(System.Type componentType, HbmMapping mapDoc, HbmNestedCompositeElement component, MemberInfo declaringComponentMember)
		{
			_componentType = componentType;
			_mapDoc = mapDoc;
			_component = component;
			_accessorPropertyMapper = new AccessorPropertyMapper(declaringComponentMember.DeclaringType, declaringComponentMember.Name, x => component.access = x);
		}

		#region Implementation of IComponentElementMapper

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
			// not supported by HbmNestedCompositeElement
		}

		public void Insert(bool consideredInInsertQuery)
		{
			// not supported by HbmNestedCompositeElement
		}

		public void Lazy(bool isLazy)
		{
			// not supported by HbmNestedCompositeElement
		}

		public void Unique(bool unique)
		{
			// not supported by HbmNestedCompositeElement
		}

		public void Class(System.Type componentConcreteType)
		{
			_component.@class = componentConcreteType.GetShortClassName(_mapDoc);
		}

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmProperty { name = property.Name };
			mapping(new PropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		public void Component(MemberInfo property, Action<IComponentElementMapper> mapping)
		{
			System.Type nestedComponentType = property.GetPropertyOrFieldType();
			var hbm = new HbmNestedCompositeElement { name = property.Name, @class = nestedComponentType.GetShortClassName(_mapDoc) };
			mapping(new ComponentNestedElementMapper(nestedComponentType, _mapDoc, hbm, property));
			AddProperty(hbm);
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmManyToOne { name = property.Name };
			mapping(new ManyToOneMapper(property, hbm, _mapDoc));
			AddProperty(hbm);
		}

		#endregion

		#region IComponentElementMapper Members

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
			// not supported by HbmNestedCompositeElement
		}

		#endregion

		protected void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			_component.Items = _component.Items == null ? toAdd : _component.Items.Concat(toAdd).ToArray();
		}

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