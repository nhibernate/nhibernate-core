using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.ByteCode.Spring
{
	/// <summary>
	/// Creates a Spring for .NET backed <see cref="IProxyFactory"/> instance.
	/// </summary>
	/// <remarks>
	/// TODO: mention how to configure
	/// </remarks>
	/// <author>Erich Eichinger</author>
	public class ProxyFactoryFactory : IProxyFactoryFactory
	{
		#region IProxyFactoryFactory Members

		public IProxyFactory BuildProxyFactory()
		{
			return new ProxyFactory();
		}

		public IProxyValidator ProxyValidator
		{
			// TODO : check what this validator does?
			get { return new DynProxyTypeValidator(); }
		}

		#endregion
	}
}