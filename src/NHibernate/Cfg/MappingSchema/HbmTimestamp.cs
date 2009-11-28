namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmTimestamp : AbstractDecoratable
	{
		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}
	}
}