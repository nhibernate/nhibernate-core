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
			get { return Items != null ? Items.Cast<IEntityPropertyMapping>() : new IEntityPropertyMapping[0]; }
		}

		#endregion
	}
}