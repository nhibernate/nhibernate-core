using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	public interface ICollectionFactoryConfiguration
	{
		IFluentSessionFactoryConfiguration Through<TCollecionsFactory>() where TCollecionsFactory : ICollectionTypeFactory;
	}
}