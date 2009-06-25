using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	public interface ICollectionFactoryConfiguration
	{
		IFluentSessionFactoryConfiguration Trough<TCollecionsFactory>() where TCollecionsFactory : ICollectionTypeFactory;
	}
}