namespace NHibernate.Cfg
{
	public interface IHibernateConfiguration
	{
		string ByteCodeProviderType { get; }
		bool UseReflectionOptimizer { get; }
		ISessionFactoryConfiguration SessionFactory { get; }
	}
}
