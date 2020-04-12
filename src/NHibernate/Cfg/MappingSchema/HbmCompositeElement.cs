using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmCompositeElement : AbstractDecoratable, IComponentMapping
	{
		#region Implementation of IComponentMapping

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

		public string Name
		{
			get { return null; }
		}

		#endregion

		#region Overrides of AbstractDecoratable

		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? Array.Empty<HbmMeta>(); }
		}

		#endregion

		#region Implementation of IPropertiesContainerMapping

		[XmlIgnore]
		public IEnumerable<IEntityPropertyMapping> Properties
		{
			get { return Items != null ? Items.Cast<IEntityPropertyMapping>() : Array.Empty<IEntityPropertyMapping>(); }
		}

		#endregion
	}
}
