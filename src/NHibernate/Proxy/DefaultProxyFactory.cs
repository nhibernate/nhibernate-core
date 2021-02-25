using System;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Proxy
{
	// Since v5.2
	[Obsolete("Use StaticProxyFactory instead")]
	public class DefaultProxyFactory : AbstractProxyFactory
	{
		private readonly ProxyFactory factory = new ProxyFactory();
		protected static readonly INHibernateLogger log = NHibernateLogger.For(typeof (DefaultProxyFactory));

		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				var initializer = new DefaultLazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod, SetIdentifierMethod, ComponentIdType, session, OverridesEquals);

				object proxyInstance = IsClassProxy
										? factory.CreateProxy(PersistentClass, initializer, Interfaces)
										: factory.CreateProxy(Interfaces[0], initializer, Interfaces);

				return (INHibernateProxy) proxyInstance;
			}
			catch (Exception ex)
			{
				log.Error(ex, "Creating a proxy instance failed");
				throw new HibernateException("Creating a proxy instance failed", ex);
			}
		}

		// Since 5.3
		[Obsolete("Use ProxyFactoryExtensions.GetFieldInterceptionProxy extension method instead.")]
		public override object GetFieldInterceptionProxy(object instanceToWrap)
		{
			var interceptor = new DefaultDynamicLazyFieldInterceptor();
			return factory.CreateProxy(PersistentClass, interceptor, new[] { typeof(IFieldInterceptorAccessor) });
		}
	}
}
