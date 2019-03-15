using System;
using NHibernate.Proxy;

namespace NHibernate.Bytecode
{
	// Since v5.2
	[Obsolete("Use StaticProxyFactoryFactory instead")]
	public class DefaultProxyFactoryFactory : IProxyFactoryFactory
	{
		#region IProxyFactoryFactory Members

		public IProxyFactory BuildProxyFactory()
		{
			return new DefaultProxyFactory();
		}

		public IProxyValidator ProxyValidator
		{
			get { return new DynProxyTypeValidator(); }
		}

		public bool IsInstrumented(System.Type entityClass)
		{
			return true;
		}

		public bool IsProxy(object entity)
		{
			return entity is INHibernateProxy;
		}

		public bool IsProxy(object entity, out INHibernateProxy proxy)
		{
			return (proxy = entity as INHibernateProxy) != null;
		}

		#endregion
	}
}
