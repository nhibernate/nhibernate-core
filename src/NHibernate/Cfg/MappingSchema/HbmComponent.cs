using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmComponent : AbstractDecoratable, IEntityPropertyMapping, IComponentMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Class
		{
			get { return @class; }
		}

		public HbmParent Parent
		{
			get { return parent; }
		}

		public string EmbeddedNode
		{
			get { return node; }
		}

		public bool IsLazyProperty
		{
			get { return lazy; }
		}

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool OptimisticLock
		{
			get { return optimisticlock; }
		}

		#endregion

		#region Implementation of IPropertiesContainerMapping

		[XmlIgnore]
		public IEnumerable<IEntityPropertyMapping> Properties
		{
			get { return Items != null ? Items.Cast<IEntityPropertyMapping>() : new IEntityPropertyMapping[0]; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}

		#endregion

	}
}