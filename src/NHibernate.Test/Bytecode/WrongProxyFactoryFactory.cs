using System;
using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.Test.Bytecode
{
	public class WrongProxyFactoryFactory : IProxyFactoryFactory
	{
		public WrongProxyFactoryFactory()
		{
			throw new System.Exception();
		}

		#region Implementation of IProxyFactoryFactory

		public IProxyFactory BuildProxyFactory()
		{
			throw new System.NotImplementedException();
		}

		public IProxyValidator ProxyValidator
		{
			get { throw new System.NotImplementedException(); }
		}

		public bool IsInstrumented(System.Type entityClass)
		{
			return false;
		}

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

		public bool IsProxy(object entity, out INHibernateProxy proxy) => (proxy = entity as INHibernateProxy) != null;

		#endregion
	}
}
