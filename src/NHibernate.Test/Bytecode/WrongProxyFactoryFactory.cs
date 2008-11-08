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

		#endregion
	}
}