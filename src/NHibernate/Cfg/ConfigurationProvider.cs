namespace NHibernate.Cfg
{
	/// <summary>
	/// Base class for NHibernate configuration settings
	/// </summary>
	public abstract class ConfigurationProvider
	{
		public abstract IHibernateConfiguration GetConfiguration();
		public abstract string GetNamedConnectionString(string name);

		/// <summary>
		/// Type that implements <see cref="INHibernateLoggerFactory"/>
		/// </summary>
		public abstract string GetLoggerFactoryClassName();
	}
}
