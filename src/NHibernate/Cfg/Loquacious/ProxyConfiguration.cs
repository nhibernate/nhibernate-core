using NHibernate.Bytecode;

namespace NHibernate.Cfg.Loquacious
{
	internal class ProxyConfiguration : IProxyConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public ProxyConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		#region Implementation of IProxyConfiguration

		public IProxyConfiguration DisableValidation()
		{
			fc.Configuration.SetProperty(Environment.UseProxyValidator, "false");
			return this;
		}

		public IFluentSessionFactoryConfiguration Through<TProxyFactoryFactory>()
			where TProxyFactoryFactory : IProxyFactoryFactory
		{
			fc.Configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
																	 typeof(TProxyFactoryFactory).AssemblyQualifiedName);
			return fc;
		}

		#endregion
	}

	internal class ProxyConfigurationProperties: IProxyConfigurationProperties
	{
		private readonly Configuration configuration;

		public ProxyConfigurationProperties(Configuration configuration)
		{
			this.configuration = configuration;
		}

		#region Implementation of IProxyConfigurationProperties

		public bool Validation
		{
			set { configuration.SetProperty(Environment.UseProxyValidator, value.ToString().ToLowerInvariant()); }
		}

		public void ProxyFactoryFactory<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory
		{
			configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
																	 typeof(TProxyFactoryFactory).AssemblyQualifiedName);
		}

		#endregion
	}
}