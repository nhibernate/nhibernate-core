using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Cfg.MappingSchema
{
	public partial class HbmJoin : IEntitySqlsMapping, IPropertiesContainerMapping
	{
		#region Implementation of IEntitySqlsMapping

		public HbmLoader SqlLoader
		{
			get { return null; }
		}

		public HbmCustomSQL SqlInsert
		{
			get { return sqlinsert; }
		}

		public HbmCustomSQL SqlUpdate
		{
			get { return sqlupdate; }
		}

		public HbmCustomSQL SqlDelete
		{
			get { return sqldelete; }
		}

		#endregion

		#region Implementation of IPropertiesContainerMapping

		public IEnumerable<IEntityPropertyMapping> Properties
		{
			get { return Items != null ? Items.Cast<IEntityPropertyMapping>() : new IEntityPropertyMapping[0]; }
		}

		#endregion
	}
}