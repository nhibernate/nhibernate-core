using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Proxy
{
	public sealed class StaticProxyFactory : AbstractProxyFactory
	{
		static readonly ConcurrentDictionary<ProxyCacheEntry, Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy>> Cache =
			new ConcurrentDictionary<ProxyCacheEntry, Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy>>();

		static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(StaticProxyFactory));

		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				var cacheEntry = new ProxyCacheEntry(IsClassProxy ? PersistentClass : typeof(object), Interfaces);
				var proxyActivator = Cache.GetOrAdd(cacheEntry, pke => CreateProxyActivator(pke));
				return proxyActivator(
					new LiteLazyInitializer(EntityName, id, session, PersistentClass),
					new NHibernateProxyFactoryInfo(EntityName, PersistentClass, Interfaces, GetIdentifierMethod, SetIdentifierMethod, ComponentIdType));
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Creating a proxy instance failed");
				throw new HibernateException("Creating a proxy instance failed", ex);
			}
		}

		Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy> CreateProxyActivator(ProxyCacheEntry pke)
		{				
			var proxyBuilder = new NHibernateProxyBuilder(GetIdentifierMethod, SetIdentifierMethod, ComponentIdType, OverridesEquals);
			var type = proxyBuilder.CreateProxyType(pke.BaseType, pke.Interfaces);
			var ctor = type.GetConstructor(new[] {typeof(ILazyInitializer), typeof(NHibernateProxyFactoryInfo)});
			var li = Expression.Parameter(typeof(ILazyInitializer));
			var pf = Expression.Parameter(typeof(NHibernateProxyFactoryInfo));
			return Expression.Lambda<Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy>>(Expression.New(ctor, li, pf), li, pf).Compile();
		}

		public override object GetFieldInterceptionProxy(object instanceToWrap)
		{
			var factory = new ProxyFactory();
			var interceptor = new DefaultDynamicLazyFieldInterceptor();
			return factory.CreateProxy(PersistentClass, interceptor, new[] { typeof(IFieldInterceptorAccessor) });
		}
	}
}
