using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentAsIdMapper: IComponentAsIdMapper
	{
		private readonly IAccessorPropertyMapper accessorPropertyMapper;
		private readonly HbmCompositeId id;
		private readonly HbmMapping mapDoc;

		public ComponentAsIdMapper(System.Type componentType, MemberInfo declaringTypeMember, HbmCompositeId id, HbmMapping mapDoc)
		{
			this.id = id;
			this.mapDoc = mapDoc;
			id.@class = componentType.GetShortClassName(mapDoc);
			id.name = declaringTypeMember.Name;
			accessorPropertyMapper = new AccessorPropertyMapper(declaringTypeMember.DeclaringType, declaringTypeMember.Name, x => id.access = x);
		}

		public void UnsavedValue(UnsavedValueType unsavedValueType)
		{
			id.unsavedvalue = (HbmUnsavedValueType)Enum.Parse(typeof(HbmUnsavedValueType), unsavedValueType.ToString());
		}

		public HbmCompositeId CompositeId
		{
			get { return id; }
		}

		public void Access(Accessor accessor)
		{
			accessorPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			accessorPropertyMapper.Access(accessorType);
		}

		public void Class(System.Type componentType)
		{
			id.@class = componentType.GetShortClassName(mapDoc);
		}

		public void Property(MemberInfo property, Action<IPropertyMapper> mapping)
		{
			var hbmProperty = new HbmKeyProperty { name = property.Name };
			mapping(new KeyPropertyMapper(property, hbmProperty));
			AddProperty(hbmProperty);
		}

		public void ManyToOne(MemberInfo property, Action<IManyToOneMapper> mapping)
		{
			var hbm = new HbmKeyManyToOne { name = property.Name };
			mapping(new KeyManyToOneMapper(property, hbm, mapDoc));
			AddProperty(hbm);
		}

		protected void AddProperty(object property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			var toAdd = new[] { property };
			id.Items = id.Items == null ? toAdd : id.Items.Concat(toAdd).ToArray();
		}
	}
}