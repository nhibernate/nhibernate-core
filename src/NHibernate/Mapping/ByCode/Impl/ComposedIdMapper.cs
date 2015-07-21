using System;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComposedIdMapper : IComposedIdMapper
	{
		private readonly System.Type container;
		private readonly HbmCompositeId id;
		private readonly HbmMapping mapDoc;

		public ComposedIdMapper(System.Type container, HbmCompositeId id, HbmMapping mapDoc)
		{
			this.container = container;
			this.id = id;
			this.mapDoc = mapDoc;
		}

		public HbmCompositeId ComposedId
		{
			get { return id; }
		}

		#region IComposedIdMapper Members

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
			id.Items = id.Items == null ? toAdd : id.Items.Concat(toAdd).ToArray();
		}
	}
}