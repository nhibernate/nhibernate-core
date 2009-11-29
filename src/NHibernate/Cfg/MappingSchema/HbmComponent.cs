using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmComponent : IEntityPropertyMapping, IPropertiesContainerMapping
	{
		#region Implementation of IEntityPropertyMapping

		public string Name
		{
			get { return name; }
		}

		#endregion

		#region Implementation of IPropertiesContainerMapping

		public IEnumerable<IEntityPropertyMapping> Properties
		{
			get { return Items.Cast<IEntityPropertyMapping>(); }
		}

		#endregion
	}
}