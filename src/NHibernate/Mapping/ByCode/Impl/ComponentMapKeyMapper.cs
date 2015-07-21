using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentMapKeyMapper : IComponentMapKeyMapper
	{
		private readonly HbmCompositeMapKey component;
		private readonly HbmMapping mapDoc;

		public ComponentMapKeyMapper(System.Type componentType, HbmCompositeMapKey component, HbmMapping mapDoc)
		{
			this.component = component;
			this.mapDoc = mapDoc;
			component.@class = componentType.GetShortClassName(mapDoc);
		}

		public HbmCompositeMapKey CompositeMapKeyMapping
		{
			get { return component; }
		}

		#region IComponentMapKeyMapper Members

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmKeyProperty {name = property.Name};
			mapping(new KeyPropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmKeyManyToOne {name = property.Name};
			mapping(new KeyManyToOneMapper(property, hbm, mapDoc));
			AddProperty(hbm);
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
	}
}