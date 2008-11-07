using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.ByteCode.LinFu
{
	public class ProxyFactoryFactory : IProxyFactoryFactory
	{
		#region IProxyFactoryFactory Members

		public IProxyFactory BuildProxyFactory()
		{
			return new ProxyFactory();
		}

		public IProxyValidator ProxyValidator
		{
			// TODO : LinFu need a specific proxy validator because need virtual methods even when we are using an interface as proxy
			get { return new DynProxyTypeValidator(); }
		}

		#endregion
	}
}