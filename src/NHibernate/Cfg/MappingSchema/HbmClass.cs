namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmClass
	{
		public HbmCacheType Cache
		{
			get { return Item; }
		}

		public HbmId Id
		{
			get { return Item1 as HbmId; }
		}

		public HbmCompositeId CompositeId
		{
			get { return Item1 as HbmCompositeId; }
		}
	}
}