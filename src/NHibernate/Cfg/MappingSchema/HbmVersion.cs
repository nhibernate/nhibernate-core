namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmVersion : AbstractDecoratable
	{
		protected override HbmMeta[] Metadatas
		{
			get { return meta ?? new HbmMeta[0]; }
		}
	}
}