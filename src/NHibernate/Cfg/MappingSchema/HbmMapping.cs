namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmMapping : HbmBase
	{
		public HbmDatabaseObject[] ListDatabaseObjects()
		{
			return databaseobject ?? new HbmDatabaseObject[0];
		}

		public HbmFilterDef[] ListFilterDefs()
		{
			return filterdef ?? new HbmFilterDef[0];
		}
	}
}