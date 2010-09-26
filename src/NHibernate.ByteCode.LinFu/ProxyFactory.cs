using System;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHibernate.ByteCode.LinFu
{
	public class ProxyFactory : AbstractProxyFactory
	{
		private static readonly global::LinFu.DynamicProxy.ProxyFactory factory = new global::LinFu.DynamicProxy.ProxyFactory();
		protected static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (ProxyFactory));

		#region IProxyFactory Members

		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				var initializer = new LazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod, SetIdentifierMethod,
				                                      ComponentIdType, session);

				object proxyInstance = IsClassProxy ? factory.CreateProxy(PersistentClass, initializer, Interfaces)
					: factory.CreateProxy(Interfaces[0], initializer, Interfaces);

				return (INHibernateProxy) proxyInstance;
			}
			catch (Exception ex)
			{
				log.Error("Creating a proxy instance failed", ex);
				throw new HibernateException("Creating a proxy instance failed", ex);
			}
		}

		#endregion
	}
}