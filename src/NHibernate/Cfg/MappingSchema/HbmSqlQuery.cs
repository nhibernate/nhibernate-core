namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmSqlQuery : HbmBase
	{
		public string GetText()
		{
			return JoinString(Text);
		}
	}
}