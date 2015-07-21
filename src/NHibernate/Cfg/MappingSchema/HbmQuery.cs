namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmQuery : HbmBase
	{
		public string GetText()
		{
			return JoinString(Text);
		}
	}
}