using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	public interface IProxyConfiguration
	{
		IProxyConfiguration DisableValidation();
		IFluentSessionFactoryConfiguration Through<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory;
	}

	public interface IProxyConfigurationProperties
	{
		bool Validation { set; }
		void ProxyFactoryFactory<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory;
	}
}