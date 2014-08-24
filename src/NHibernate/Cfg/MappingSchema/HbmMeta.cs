namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmMeta : HbmBase
	{
		public string GetText()
		{
			return JoinString(Text);
		}
	}
}