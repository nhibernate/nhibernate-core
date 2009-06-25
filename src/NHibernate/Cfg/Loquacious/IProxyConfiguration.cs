using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	public interface IProxyConfiguration
	{
		IProxyConfiguration DisableValidation();
		IFluentSessionFactoryConfiguration Trough<TProxyFactoryFactory>() where TProxyFactoryFactory : IProxyFactoryFactory;
	}
}