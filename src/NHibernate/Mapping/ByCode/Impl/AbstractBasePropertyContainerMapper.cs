using System;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public abstract class AbstractBasePropertyContainerMapper
	{
		protected System.Type container;
		protected HbmMapping mapDoc;

		protected AbstractBasePropertyContainerMapper(System.Type container, HbmMapping mapDoc)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			if (mapDoc == null)
			{
				throw new ArgumentNullException("mapDoc");
			}
			this.container = container;
			this.mapDoc = mapDoc;
		}

		protected HbmMapping MapDoc
		{
			get { return mapDoc; }
		}

		protected System.Type Container
		{
			get { return container; }
		}

		protected abstract void AddProperty(object property);

		public virtual void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			if (!IsMemberSupportedByMappedContainer(property))
			{
				throw new ArgumentOutOfRangeException("property", "Can't add a property of another graph");
			}
			var hbmProperty = new HbmProperty {name = property.Name};
			mapping(new PropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		protected virtual bool IsMemberSupportedByMappedContainer(MemberInfo property)
		{
			return property.DeclaringType.IsAssignableFrom(container);
		}

		public virtual void Component(MemberInfo property, Action<IComponentMapper> mapping)
		{
			if (!IsMemberSupportedByMappedContainer(property))
			{
				throw new ArgumentOutOfRangeException("property", "Can't add a property of another graph");
			}
			var hbm = new HbmComponent { name = property.Name };
			mapping(new ComponentMapper(hbm, property.GetPropertyOrFieldType(), property, MapDoc));
			AddProperty(hbm);
		}

		public virtual void Component(MemberInfo property, Action<IDynamicComponentMapper> mapping)
		{
			if (!IsMemberSupportedByMappedContainer(property))
			{
				throw new ArgumentOutOfRangeException("property", "Can't add a property of another graph");
			}
			var hbm = new HbmDynamicComponent { name = property.Name };
			mapping(new DynamicComponentMapper(hbm, property, MapDoc));
			AddProperty(hbm);
		}

		public virtual void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			if (!IsMemberSupportedByMappedContainer(property))
			{
				throw new ArgumentOutOfRangeException("property", "Can't add a property of another graph");
			}
			var hbm = new HbmManyToOne { name = property.Name };
			mapping(new ManyToOneMapper(property, hbm, MapDoc));
			AddProperty(hbm);
		}

		public virtual void Any(MemberInfo property, System.Type idTypeOfMetaType, Action<IAnyMapper> mapping)
		{
			if (!IsMemberSupportedByMappedContainer(property))
			{
				throw new ArgumentOutOfRangeException("property", "Can't add a property of another graph");
			}
			var hbm = new HbmAny { name = property.Name };
			mapping(new AnyMapper(property, idTypeOfMetaType, hbm, MapDoc));
			AddProperty(hbm);
		}
	}
}