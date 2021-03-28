namespace NHibernate.Cfg
{
	class NullConfigurationProvider : ConfigurationProvider
	{
		public override IHibernateConfiguration GetConfiguration()
		{
			return null;
		}

		public override string GetNamedConnectionString(string name)
		{
			return null;
		}

		public override string GetLoggerFactoryClassName()
		{
			return null;
		}
	}
}
