using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmProperties : AbstractDecoratable, IEntityPropertyMapping, IComponentMapping
	{
		#region Implementation of IEntityPropertyMapping

		public bool IsLazyProperty
		{
			get { return false; }
		}

		public string Class
		{
			get { return null; }
		}

		public HbmParent Parent
		{
			get { return null; }
		}

		public string EmbeddedNode
		{
			get { return node; }
		}

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return "embedded"; }
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
			get { return new HbmMeta[0]; }
		}

		#endregion
	}
}
