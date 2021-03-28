namespace NHibernate.Cfg
{
	/// <summary>
	/// Base class for NHibernate configuration settings
	/// </summary>
	public abstract class ConfigurationProvider
	{
		private static ConfigurationProvider _current = new StaticConfigurationManagerProvider();

		/// <summary>
		/// Provides ability to override default <see cref="System.Configuration.ConfigurationManager"/> with custom implementation.
		/// Can be set to null if all configuration is specified by code 
		/// </summary>
		public static ConfigurationProvider Current
		{
			get => _current;
			set => _current = value ?? new NullConfigurationProvider();
		}

		public abstract IHibernateConfiguration GetConfiguration();

		public abstract string GetNamedConnectionString(string name);

		/// <summary>
		/// Type that implements <see cref="INHibernateLoggerFactory"/>
		/// </summary>
		public abstract string GetLoggerFactoryClassName();
	}
}
