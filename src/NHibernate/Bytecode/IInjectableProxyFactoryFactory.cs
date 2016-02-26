namespace NHibernate.Bytecode
{
	// To support http://nhibernate.jira.com/browse/NH-975
	// We give to the user the ability to configure the proxyFactoryFactory class via
	// session-factory configuration.
	public interface IInjectableProxyFactoryFactory
	{
		void SetProxyFactoryFactory(string typeName);
	}
}