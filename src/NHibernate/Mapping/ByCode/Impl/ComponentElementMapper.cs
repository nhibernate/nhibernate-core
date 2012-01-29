using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentElementMapper : IComponentElementMapper
	{
		private readonly HbmCompositeElement component;
		private readonly System.Type componentType;
		protected readonly HbmMapping mapDoc;
		private IComponentParentMapper parentMapper;

		public ComponentElementMapper(System.Type componentType, HbmMapping mapDoc, HbmCompositeElement component)
		{
			this.componentType = componentType;
			this.mapDoc = mapDoc;
			this.component = component;
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
			// not supported by HbmCompositeElement
		}

		public void Insert(bool consideredInInsertQuery)
		{
			// not supported by HbmCompositeElement
		}

		public void Lazy(bool isLazy)
		{
			// not supported by HbmCompositeElement
		}

		public void Unique(bool unique)
		{
			// not supported by HbmCompositeElement
		}

		public void Class(System.Type componentConcreteType)
		{
			component.@class = componentConcreteType.GetShortClassName(mapDoc);
		}

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmProperty {name = property.Name};
			mapping(new PropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		public void Component(MemberInfo property, Action<IComponentElementMapper> mapping)
		{
			System.Type nestedComponentType = property.GetPropertyOrFieldType();
			var hbm = new HbmNestedCompositeElement
					  {name = property.Name, @class = nestedComponentType.GetShortClassName(mapDoc)};
			mapping(new ComponentNestedElementMapper(nestedComponentType, mapDoc, hbm, property));
			AddProperty(hbm);
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmManyToOne {name = property.Name};
			mapping(new ManyToOneMapper(property, hbm, mapDoc));
			AddProperty(hbm);
		}

		#endregion

		#region IComponentElementMapper Members

		public void Access(Accessor accessor)
		{
			// not supported by HbmCompositeElement
		}

		public void Access(System.Type accessorType)
		{
			// not supported by HbmCompositeElement
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			// not supported by HbmCompositeElement
		}

		#endregion

		protected void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] {property};
			component.Items = component.Items == null ? toAdd : component.Items.Concat(toAdd).ToArray();
		}

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