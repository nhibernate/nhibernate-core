namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmParam : HbmBase
	{
		public string GetText()
		{
			return JoinString(Text);
		}
	}
}