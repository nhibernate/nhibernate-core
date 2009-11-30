using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmComponent : AbstractDecoratable, IEntityPropertyMapping, IPropertiesContainerMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		public string Access
		{
			get { return access; }
		}

		public bool OptimisticKock
		{
			get { return optimisticlock; }
		}

		#endregion

		#region Implementation of IPropertiesContainerMapping

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