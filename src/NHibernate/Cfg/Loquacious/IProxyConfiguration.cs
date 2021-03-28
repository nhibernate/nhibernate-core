using System;
using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IProxyConfiguration
	{
		IProxyConfiguration DisableValidation();
		IFluentSessionFactoryConfiguration Through<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory;
	}

	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IProxyConfigurationProperties
	{
		bool Validation { set; }
		void ProxyFactoryFactory<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory;
	}
}
