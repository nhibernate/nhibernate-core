using NHibernate.Proxy;
using NHibernate.Proxy.Poco.Castle;

namespace NHibernate.Bytecode
{
	public class DefaultProxyFactoryFactory : IProxyFactoryFactory
	{
		#region IProxyFactoryFactory Members

		public IProxyFactory BuildProxyFactory()
		{
			return new CastleProxyFactory();
		}

		#endregion
	}
}
