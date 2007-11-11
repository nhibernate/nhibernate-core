using System;
using NHibernate.Proxy;
using NHibernate.Proxy.Poco.Castle;

namespace NHibernate.Bytecode
{
	public class DefaultProxyFactoryFactory : IProxyFactoryFactory
	{
		private readonly System.Type proxyFactoryClass;

		public DefaultProxyFactoryFactory() {}

		public DefaultProxyFactoryFactory(System.Type proxyFactoryClass)
		{
			this.proxyFactoryClass = proxyFactoryClass;
		}

		#region IProxyFactoryFactory Members

		public IProxyFactory BuildProxyFactory()
		{
			if (proxyFactoryClass == null || proxyFactoryClass == typeof(CastleProxyFactory))
				return new CastleProxyFactory();
			try
			{
				return (IProxyFactory)Activator.CreateInstance(proxyFactoryClass);
			}
			catch (Exception e)
			{
				throw new HibernateException("Failed to create an instance of '" + proxyFactoryClass.FullName + "'!", e);
			}
		}

		#endregion
	}
}
