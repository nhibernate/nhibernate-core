using NHibernate.Bytecode;

namespace NHibernate.Cfg.Loquacious
{
	public class ProxyConfiguration
#pragma warning disable 618
		: IProxyConfiguration
#pragma warning restore 618
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public ProxyConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		public ProxyConfiguration DisableValidation()
		{
			fc.Configuration.SetProperty(Environment.UseProxyValidator, "false");
			return this;
		}

		public FluentSessionFactoryConfiguration Through<TProxyFactoryFactory>()
			where TProxyFactoryFactory : IProxyFactoryFactory
		{
			fc.Configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
										typeof(TProxyFactoryFactory).AssemblyQualifiedName);
			return fc;
		}

		#region Implementation of IProxyConfiguration
#pragma warning disable 618

		IProxyConfiguration IProxyConfiguration.DisableValidation()
		{
			return DisableValidation();
		}

		IFluentSessionFactoryConfiguration IProxyConfiguration.Through<TProxyFactoryFactory>()
		{
			return Through<TProxyFactoryFactory>();
		}

#pragma warning restore 618
		#endregion
	}

	public class ProxyConfigurationProperties
#pragma warning disable 618
		: IProxyConfigurationProperties
#pragma warning restore 618
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
