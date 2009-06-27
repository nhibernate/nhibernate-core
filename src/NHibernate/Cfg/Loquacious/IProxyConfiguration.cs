using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	public interface IProxyConfiguration
	{
		IProxyConfiguration DisableValidation();
		IFluentSessionFactoryConfiguration Through<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory;
	}
}