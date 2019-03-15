using NHibernate.Proxy;

namespace NHibernate.Bytecode
{
	public class StaticProxyFactoryFactory : IProxyFactoryFactory
	{
		internal static StaticProxyFactoryFactory Instance = new StaticProxyFactoryFactory();

		public IProxyFactory BuildProxyFactory() => new StaticProxyFactory();

		public IProxyValidator ProxyValidator => new DynProxyTypeValidator();

		public bool IsInstrumented(System.Type entityClass) => true;

		public bool IsProxy(object entity) => entity is INHibernateProxy;

		public bool IsProxy(object entity, out INHibernateProxy proxy) => (proxy = entity as INHibernateProxy) != null;
	}
}
