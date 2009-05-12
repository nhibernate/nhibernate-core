using System;

namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmClass: AbstractDecoratable
	{
		public HbmId Id
		{
			get { return Item as HbmId; }
		}

		public HbmCompositeId CompositeId
		{
			get { return Item as HbmCompositeId; }
		}

		public HbmVersion Version
		{
			get { return Item1 as HbmVersion; }
		}

		public HbmTimestamp Timestamp
		{
			get { return Item1 as HbmTimestamp; }
		}

		protected override HbmMeta[] GetMetadataField()
		{
			return meta;
		}
	}
}