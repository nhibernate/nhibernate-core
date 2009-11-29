using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmNaturalId: IPropertiesContainerMapping
	{
		#region Implementation of IPropertiesContainerMapping

		public IEnumerable<IEntityPropertyMapping> Properties
		{
			get { return Items.Cast<IEntityPropertyMapping>(); }
		}

		#endregion
	}
}