namespace NHibernate.Cfg.MappingSchema
{
	partial class HbmFilterDef : HbmBase
	{
		public string GetDefaultCondition()
		{
			return JoinString(Text) ?? condition;
		}

		public HbmFilterParam[] ListParameters()
		{
			return filterparam ?? new HbmFilterParam[0];
		}
	}
}